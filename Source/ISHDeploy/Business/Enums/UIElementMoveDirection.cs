﻿/*
 * Copyright (c) 2014 All Rights Reserved by the SDL Group.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace ISHDeploy.Business.Enums
{
    /// <summary>
    /// Directions to move an item to change an order in list of UI elements.
    ///	<para type="description">Enumeration of directions to move an item to change an order in list of UI elements.</para>
    /// </summary>
    public enum UIElementMoveDirection
    {
        /// <summary>
        /// Move to first position in list.
        /// </summary>
        First,

        /// <summary>
        /// Move to last position in list.
        /// </summary>
        Last,

        /// <summary>
        /// Move to the position after.
        /// </summary>
        After
    }
}