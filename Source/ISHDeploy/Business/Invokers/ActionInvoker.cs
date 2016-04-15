﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using ISHDeploy.Interfaces;
using ISHDeploy.Interfaces.Actions;

namespace ISHDeploy.Business.Invokers
{
    /// <summary>
    /// Executes the sequence of actions one by one.
    /// </summary>
    public class ActionInvoker : IActionInvoker
    {
        /// <summary>
        /// The identifier if progress needs to be shown.
        /// </summary>
        private readonly bool _showProgress;

        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Description for general activity that to be done.
        /// </summary>
        private readonly string _activityDescription;

        /// <summary>
        /// Sequence of the actions that <see cref="T:ISHDeploy.Business.Invokers.ActionInvoker"/> going to execute
        /// </summary>
        private readonly List<IAction> _actions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionInvoker"/> class.
        /// </summary>
        /// <param name="logger">Instance of the <see cref="T:ISHDeploy.Interfaces.ILogger"/></param>
		/// <param name="activityDescription">Description of the general activity to be done</param>
		/// <param name="showProgress">Defines if progress should be shown. By default is false</param>
        public ActionInvoker(ILogger logger, string activityDescription, bool showProgress = false)
        {
            _logger = logger;
		    _showProgress = showProgress;
            _activityDescription = activityDescription;
            _actions = new List<IAction>();
        }

        /// <summary>
        /// Adds action to the sequence.
        /// </summary>
        /// <param name="action">New action in the sequence.</param>
        public void AddAction(IAction action)
        {
            Debug.Assert(action != null, "Action cannot be null");
            _actions.Add(action);
        }

        /// <summary>
        /// Executes sequence of actions one by one.
        /// </summary>
        public void Invoke()
        {
            _logger.WriteDebug($"Entered Invoke method for `{nameof(ActionInvoker)}`");

			List<IAction> executedActions = new List<IAction>();
            try
            {
                for (var i = 0; i < _actions.Count; i++)
                {
                    var action = _actions[i];

                    action.Execute();

					// If action was executed, we`re adding it to the list to make a rollback if necessary;
					executedActions.Add(action);

                    if (_showProgress)
                    {
                        var actionNumber = i + 1;
                        _logger.WriteProgress(_activityDescription, $"Executed {actionNumber} of {_actions.Count} actions", (int)(actionNumber / (double)_actions.Count * 100));
                    }
                }

                _logger.WriteVerbose($"`{_activityDescription}` completed");
            }
            catch
            {
				//	To do a rollback we need to do it in a sequence it was executed, thus we should reverse list.

				//	We are doing rollback by default, as if we would want to do in on demand,
				//	then we`d have to add a flag into method invocation
				{
					executedActions.Reverse();
					executedActions.ForEach(x =>
					{
						(x as IRestorableAction)?.Rollback();
					});
                }

                throw;
            }
	        finally
	        {
				// We need to dispose the command where it`s possible as it cleans backed up assets
				_actions.ForEach(x =>
				{
					(x as IDisposable)?.Dispose();
				});
	        }
        }
    }
}
