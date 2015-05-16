﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotGame.Rendering.Passes
{
    /// <summary>
    /// Stellt einen Pass dar, welcher DeferredShading als Rendertechnik benutzt.
    /// </summary>
    public class DeferredPass : Pass
    {
        public DeferredPass(Engine engine)
            : base(engine, null)
        {

        }

        public override void Apply(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool isDisposing)
        {
            throw new NotImplementedException();
        }
    }
}