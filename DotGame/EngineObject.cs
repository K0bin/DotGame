﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotGame
{
    public abstract class EngineObject : IDisposable
    {
        public Engine Engine { get; private set; }
        public object Tag { get; set; }

        public EngineObject(Engine engine)
        {
            if (engine == null)
                throw new ArgumentNullException("engine");

            this.Engine = engine;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected abstract void Dispose(bool isDisposing);
    }
}