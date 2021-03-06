﻿// ************************************************************************************
// <copyright file="WeakAction.cs" company="GalaSoft Laurent Bugnion">
// Copyright © GalaSoft Laurent Bugnion 2009-2014
// </copyright>
// ************************************************************************************
// <author>Laurent Bugnion</author>
// <email>laurent@galasoft.ch</email>
// <date>18.9.2009</date>
// <project>GalaSoft.MvvmLight</project>
// <web>http://www.galasoft.ch</web>
// <license>
// The work is originally forked / taken from GalaSoft.MvvmLight toolkit codebase 
// which is licensed under MIT license and remains copyright work of Laurent Bugnion.
// See license.txt in this project or visit http://www.galasoft.ch/license_MIT.txt
// </license>
// ************************************************************************************

namespace DotNetStuffs.SimpleEvents
{
    #region Namespace

    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    
    #endregion

    /// <summary>
    /// Stores an <see cref="Action" /> without causing a hard reference to be created to the Action's owner. The owner can be garbage collected at any time.
    /// </summary>
    public class WeakAction
    {
        #region Variable declaration

        /// <summary>
        /// Instance of static action.
        /// </summary>
        private Action staticAction;
        
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakAction" /> class.
        /// </summary>
        /// <param name="action">The action that will be associated to this instance.</param>
        public WeakAction(Action action)
            : this(null == action ? null : action.Target, action)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakAction" /> class.
        /// </summary>
        /// <param name="target">The action's owner.</param>
        /// <param name="action">The action that will be associated to this instance.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Method should fail with an exception if action is null.")]
        public WeakAction(object target, Action action)
        {
            if (action.Method.IsStatic)
            {
                this.staticAction = action;
                if (null != target)
                {
                    // Keep a reference to the target to control the WeakAction's lifetime.
                    this.Reference = new WeakReference(target);
                }

                return;
            }

            this.Method = action.Method;
            this.ActionReference = new WeakReference(action.Target);
            this.Reference = new WeakReference(target);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakAction" /> class.
        /// </summary>
        protected WeakAction()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the method that this WeakAction represents.
        /// </summary>
        public virtual string MethodName
        {
            get
            {
                return null != this.staticAction ? this.staticAction.Method.Name : this.Method.Name;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Action's owner is still alive, or if it was collected by the Garbage Collector already.
        /// </summary>
        public virtual bool IsAlive
        {
            get
            {
                if (null == this.staticAction && null == this.Reference)
                {
                    return false;
                }

                if (null == this.staticAction)
                {
                    return this.Reference.IsAlive;
                }

                return null == this.Reference || this.Reference.IsAlive;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the WeakAction is static or not.
        /// </summary>
        public bool IsStatic
        {
            get
            {
                return null != this.staticAction;
            }
        }

        /// <summary>
        /// Gets the Action's owner. This object is stored as a 
        /// <see cref="WeakReference" />.
        /// </summary>
        public object Target
        {
            get
            {
                return null == this.Reference ? null : this.Reference.Target;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="MethodInfo" /> corresponding to this WeakAction's
        /// method passed in the constructor.
        /// </summary>
        protected MethodInfo Method
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a WeakReference to this WeakAction's action's target.
        /// This is not necessarily the same as
        /// <see cref="Reference" />, for example if the
        /// method is anonymous.
        /// </summary>
        protected WeakReference ActionReference
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a WeakReference to the target passed when constructing
        /// the WeakAction. This is not necessarily the same as
        /// <see cref="ActionReference" />, for example if the
        /// method is anonymous.
        /// </summary>
        protected WeakReference Reference
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the target of the weak reference.
        /// </summary>
        protected object ActionTarget
        {
            get
            {
                return null == this.ActionReference ? null : this.ActionReference.Target;
            }
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Executes the action. This only happens if the action's owner
        /// is still alive.
        /// </summary>
        public void Execute()
        {
            if (null != this.staticAction)
            {
                this.staticAction();
                return;
            }

            var actionTarget = this.ActionTarget;
            if (!this.IsAlive)
            {
                return;
            }

            if (null != this.Method && null != this.ActionReference && null != actionTarget)
            {
                this.Method.Invoke(actionTarget, null);
            }
        }

        /// <summary>
        /// Sets the reference that this instance stores to null.
        /// </summary>
        public void MarkForDeletion()
        {
            this.Reference = null;
            this.ActionReference = null;
            this.Method = null;
            this.staticAction = null;
        }
        
        #endregion
    }
}