/*
 * Copyright 2009 Daniel H�lbling - http://www.tigraine.at
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 *  
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace Pandora
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Practices.ServiceLocation;

    public class CommonServiceLocatorAdapter : ServiceLocatorImplBase
    {
        private readonly IPandoraContainer container;

        public CommonServiceLocatorAdapter(IPandoraContainer container)
        {
            this.container = container;
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            return container.Resolve(serviceType, key);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return container.ResolveAll(serviceType);
        }
    }
}