﻿using System;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHUIEventMonitorTab
{
	/// <summary>
	/// Removes Event Monitor Tab".
	/// </summary>
	/// <seealso cref="IOperation" />
	public class MoveISHUIEventMonitorTabOperation : IOperation
    {
		/// <summary>
		/// Operation type enum
		/// </summary>
		public enum OperationType
		{

			/// <summary>
			/// Flag to insert after 
			/// </summary>
			InsertAfter,

			/// <summary>
			/// Flag to insert before
			/// </summary>
			InsertBefore
		}

        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

		/// <summary>
		/// Initializes a new instance of the <see cref="RemoveISHUIEventMonitorTabOperation" /> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="paths">Reference for all files paths.</param>
		/// <param name="label">Label of the element</param>
		/// <param name="operationType">Type of the operation.</param>
		/// <param name="targetLabel">The target label.</param>
		public MoveISHUIEventMonitorTabOperation(ILogger logger, ISHPaths paths, string label, OperationType operationType, string targetLabel = null)
        {
            _invoker = new ActionInvoker(logger, "Removing Event Monitor Tab");

			string nodeXPath = String.Format(CommentPatterns.EventMonitorTab, label);
			string nodeCommentXPath = nodeXPath + CommentPatterns.EventMonitorPreccedingCommentXPath;

			string targetNodeXPath = String.IsNullOrEmpty(targetLabel) ? null :  String.Format(CommentPatterns.EventMonitorTab, targetLabel);

			// Combile node and its xPath
			string nodesToMoveXPath = nodeXPath + "|" + nodeCommentXPath;

			switch (operationType)
	        {
				case OperationType.InsertAfter:
					_invoker.AddAction(new MoveAfterNodeAction(logger, paths.EventMonitorMenuBar, nodesToMoveXPath, targetNodeXPath));
					break;
				case OperationType.InsertBefore:
					_invoker.AddAction(new MoveBeforeNodeAction(logger, paths.EventMonitorMenuBar, nodesToMoveXPath, targetNodeXPath));
					break;
			}
		}

        /// <summary>
        /// Runs current operation.
        /// </summary>
        public void Run()
        {
            _invoker.Invoke();
        }
    }
}