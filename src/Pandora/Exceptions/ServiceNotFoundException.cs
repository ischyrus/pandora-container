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

    public class ServiceNotFoundException : ApplicationException
    {
        public ServiceNotFoundException(Type type)
            : base(String.Format("No service for type {0} could be found", type.FullName))
        {
        }

        public ServiceNotFoundException(Type type, string name)
            :base(String.Format("No service for type {0} named {1} could be found", type.FullName, name))
        {
            
        }
    }
}