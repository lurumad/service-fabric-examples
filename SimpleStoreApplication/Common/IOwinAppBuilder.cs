﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Owin;

namespace Common
{
    public interface IOwinAppBuilder
    {
        void Configuration(IAppBuilder appBuilder);
    }
}