﻿using System;
using System.Collections.Generic;
using System.Text;

namespace IoTBackend.Core.Configurations
{
    public class BlobConfiguration
    {
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
    }
}
