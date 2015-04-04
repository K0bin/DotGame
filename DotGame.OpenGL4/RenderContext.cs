﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DotGame.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace DotGame.OpenGL4
{
    internal class RenderContext : GraphicsObject, IRenderContext
    {
        private RenderStateInfo currentState = new RenderStateInfo();
        private VertexBuffer currentVertexBuffer;
        private IndexBuffer currentIndexBuffer;

        private bool stateDirty;
        private bool vertexBufferDirty;
        private bool indexBufferDirty;

        private Color clearColor;
        private float clearDepth;
        private int clearStencil;

        public RenderContext(GraphicsDevice graphicsDevice)
            : base(graphicsDevice, new System.Diagnostics.StackTrace(1))
        {

        }

        public void Update<T>(IConstantBuffer buffer, T data) where T : struct
        {
            var internalBuffer = graphicsDevice.Cast<ConstantBuffer>(buffer, "buffer");
            if (internalBuffer.Size < 0)
                internalBuffer.Size = Marshal.SizeOf(data);

            graphicsDevice.StateManager.ConstantBuffer = buffer;
            // TODO (Robin) BufferUsageHint
            GL.BufferData<T>(BufferTarget.UniformBuffer, new IntPtr(buffer.Size), ref data, BufferUsageHint.DynamicDraw);
            OpenGL4.GraphicsDevice.CheckGLError();
        }

        public void Update<T>(ITexture2D texture, T[] data) where T : struct
        {
            Update(texture, 0, data);
        }

        public void Update<T>(ITexture2D texture, int mipLevel, T[] data) where T : struct
        {
            if (texture == null)
                throw new ArgumentNullException("texture");
            if (mipLevel < 0 || mipLevel >= texture.MipLevels)
                throw new ArgumentOutOfRangeException("mipLevel");
        
            throw new NotImplementedException();
        }

        public void Update<T>(ITexture2DArray textureArray, int arrayIndex, T[] data) where T : struct
        {
            Update(textureArray, arrayIndex, 0, data);
        }

        public void Update<T>(ITexture2DArray textureArray, int arrayIndex, int mipLevel, T[] data) where T : struct
        {
            if (textureArray == null)
                throw new ArgumentNullException("texture");
            if (arrayIndex < 0 || arrayIndex >= textureArray.ArraySize)
                throw new ArgumentOutOfRangeException("arrayIndex");
            if (mipLevel < 0 || mipLevel >= textureArray.MipLevels)
                throw new ArgumentOutOfRangeException("mipLevel");

            throw new NotImplementedException();
        }

        public void GenerateMips(ITexture2D texture)
        {
            if (texture == null)
                throw new ArgumentNullException("texture");
            throw new NotImplementedException();
        }

        public void GenerateMips(ITexture2DArray textureArray)
        {
            if (textureArray == null)
                throw new ArgumentNullException("textureArray");

            throw new NotImplementedException();
        }
        

        public void SetRenderTarget(IRenderTarget2D color, IRenderTarget2D depth)
        {
            throw new NotImplementedException();
        }

        public void SetRenderTargetColor(IRenderTarget2D color)
        {
            throw new NotImplementedException();
        }

        public void SetRenderTargetDepth(IRenderTarget2D depth)
        {
            throw new NotImplementedException();
        }

        public void Clear(Color color)
        {
            Clear(ClearOptions.ColorDepth, color, 1.0f, 0);
        }

        public void Clear(ClearOptions options, Color color, float depth, int stencil)
        {
            ClearBufferMask mask = ClearBufferMask.None;

            if (options.HasFlag(ClearOptions.Color))
            {
                SetClearColor(ref color);
                mask |= ClearBufferMask.ColorBufferBit;
            }

            if (options.HasFlag(ClearOptions.Depth))
            {
                SetClearDepth(ref depth);
                mask |= ClearBufferMask.DepthBufferBit;
            }

            if (options.HasFlag(ClearOptions.Stencil))
            {
                SetClearStencil(ref stencil);
                mask |= ClearBufferMask.StencilBufferBit;
            }

            GL.Clear(mask);
        }

        private void SetClearColor(ref Color color)
        {
            if (color != clearColor)
            {
                clearColor = color;
                GL.ClearColor(color.R, color.G, color.B, color.A);
            }
        }

        private void SetClearDepth(ref float depth)
        {
            if (depth != clearDepth)
            {
                clearDepth = depth;
                GL.ClearDepth(depth);
            }
        }

        private void SetClearStencil(ref int stencil)
        {
            if (stencil != clearStencil)
            {
                clearStencil = stencil;
                GL.ClearStencil(stencil);
            }
        }

        public void SetShader(IShader shader)
        {
            if (shader == null)
                throw new ArgumentNullException("shader");
            graphicsDevice.Cast<IShader>(shader, "shader"); // Shader überprüfen

            currentState.Shader = shader;
            stateDirty = true;
        }

        public void SetPrimitiveType(PrimitiveType type)
        {
            EnumConverter.Convert(type); // Type überprüfen (ob supported ist)

            currentState.PrimitiveType = type;
            stateDirty = true;
        }

        public void SetRasterizer(IRasterizerState rasterizerState)
        {
            throw new NotImplementedException();
        }

        public void SetState(IRenderState state)
        {
            if (state == null)
                throw new ArgumentNullException("state");
            //graphicsDevice.Cast<RenderState>(state, "state"); // State überprüfen

            if (!state.Info.Equals(currentState))
            {
                stateDirty = true;
                currentState = state.Info;
            }
        }

        public void SetVertexBuffer(IVertexBuffer vertexBuffer)
        {
            if (vertexBuffer == null)
                throw new ArgumentNullException("vertexBuffer");
            if (vertexBuffer.IsDisposed)
                throw new ArgumentException("VertexBuffer is disposed.", "vertexBuffer");

            if (currentVertexBuffer != vertexBuffer)
            {
                currentVertexBuffer = graphicsDevice.Cast<VertexBuffer>(vertexBuffer, "vertexBuffer");
                vertexBufferDirty = true;
            }
        }

        public void SetIndexBuffer(IIndexBuffer indexBuffer)
        {
            if (indexBuffer == null)
                throw new ArgumentNullException("indexBuffer");
            if (indexBuffer.IsDisposed)
                throw new ArgumentException("IndexBuffer is disposed.", "indexBuffer");

            if (currentIndexBuffer != indexBuffer)
            {
                currentIndexBuffer = graphicsDevice.Cast<IndexBuffer>(indexBuffer, "indexBuffer");
                indexBufferDirty = true;
            }
        }

        public void SetConstantBuffer(IShader shader, string name, IConstantBuffer buffer)
        {
            var internalShader = graphicsDevice.Cast<Shader>(shader, "shader");
            // TODO (Robin) Durch GraphicsDevice Methode ersetzen
            graphicsDevice.StateManager.Shader = shader;

            ConstantBuffer internalBuffer = graphicsDevice.Cast<ConstantBuffer>(buffer, "buffer");

            graphicsDevice.StateManager.ConstantBuffer = buffer;
            GL.BindBufferBase(BufferRangeTarget.UniformBuffer, internalShader.GetUniformBindingPoint(name), internalBuffer.UniformBufferObjectID);
            OpenGL4.GraphicsDevice.CheckGLError();
        }

        public void SetConstantBuffer(IShader shader, IConstantBuffer buffer)
        {
            SetConstantBuffer(shader, "global", buffer);
        }


        public void SetTexture(IShader shader, string name, ITexture2D texture)
        {
            var internalShader = graphicsDevice.Cast<Shader>(shader, "shader");
            
            GL.ActiveTexture(TextureUnit.Texture0 + internalShader.GetTextureUnit(name));

            Texture2D internalTexture = graphicsDevice.Cast<Texture2D>(texture, "texture");
            GL.BindTexture(TextureTarget.Texture2D, internalTexture.TextureID);

            // TODO (Robin) Texture setzen, das ist nur ein Test
        }

        public void SetTexture(IShader shader, string name, ITexture2DArray texture)
        {
            throw new NotImplementedException();
        }

        public void SetTexture(IShader shader, string name, ITexture3D texture)
        {
            throw new NotImplementedException();
        }

        public void SetTexture(IShader shader, string name, ITexture3DArray texture)
        {
            throw new NotImplementedException();
        }

        public void SetSampler(IShader shader, string name, ISampler sampler)
        {
            throw new NotImplementedException();
        }

        private void ApplyState()
        {
            var shader = graphicsDevice.Cast<Shader>(currentState.Shader, "currentState.Shader");
            if (stateDirty)
            {
                graphicsDevice.StateManager.Shader = shader;

                // TODO (henrik1235) RasterizerState einbauen
                /*if (currentState.CullMode != CullMode.None) 
                {
                    GL.Enable(EnableCap.CullFace);
                    GL.CullFace(EnumConverter.Convert(currentState.CullMode));
                }
                else
                    GL.Disable(EnableCap.CullFace);*/
            }
            if (vertexBufferDirty)
            {
                graphicsDevice.StateManager.VertexBuffer = currentVertexBuffer;

                if (currentState.Shader == null)
                    throw new Exception("No shader set!");

                Shader internalShader = graphicsDevice.Cast<Shader>(currentState.Shader, "currentState.Shader");
                if (currentVertexBuffer.Shader != currentState.Shader)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, currentVertexBuffer.VertexBufferObjectID);

                    int offset = 0;
                    VertexElement[] elements = currentVertexBuffer.Description.GetElements();
                    for (int i = 0; i < currentVertexBuffer.Description.ElementCount; i++)
                    {
                        GL.EnableVertexAttribArray(i);
                        GL.BindAttribLocation(internalShader.ProgramID, i, EnumConverter.Convert(elements[i].Usage));

                        GL.VertexAttribPointer(i, graphicsDevice.GetComponentsOf(elements[i].Type), VertexAttribPointerType.Float, false, graphicsDevice.GetSizeOf(currentVertexBuffer.Description), offset);
                        offset += graphicsDevice.GetSizeOf(elements[i].Type);
                    }
                    currentVertexBuffer.Shader = internalShader;
                }
            }
            if (indexBufferDirty)
                graphicsDevice.StateManager.IndexBuffer = currentIndexBuffer;

            //TEST
            GL.Enable(EnableCap.DepthTest);
            GL.FrontFace(FrontFaceDirection.Cw);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            OpenGL4.GraphicsDevice.CheckGLError();

            stateDirty = false;
            vertexBufferDirty = false;
            indexBufferDirty = false;
        }

        public void Draw()
        {
            if (currentVertexBuffer == null)
                throw new InvalidOperationException("Tried to draw without a vertexbuffer set.");

            ApplyState();
            GL.DrawArrays(EnumConverter.Convert(currentState.PrimitiveType), 0, currentVertexBuffer.VertexCount);
        }

        public void DrawIndexed()
        {
            if (currentVertexBuffer == null)
                throw new InvalidOperationException("Tried to draw without a vertexbuffer set.");
            if (currentIndexBuffer == null)
                throw new InvalidOperationException("Tried to draw without indexbuffer set.");

            ApplyState();
            GL.DrawElements((BeginMode)EnumConverter.Convert(currentState.PrimitiveType), currentIndexBuffer.IndexCount, EnumConverter.Convert(currentIndexBuffer.Format), 0);
        }

        protected override void Dispose(bool isDisposing)
        {

        }
    }
}