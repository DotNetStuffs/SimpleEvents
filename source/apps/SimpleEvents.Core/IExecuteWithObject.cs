// **************************************************************************
// <copyright file="IExecuteWithObject.cs" company="Jsinh.in">
// Copyright © Jsinh 2014
// </copyright>
// ****************************************************************************
// <author>Jaspalsinh Chauhan</author>
// <email>jachauhan@gmail.com</email>
// <date>25.01.2014</date>
// <project>Jsinh.Messaging</project>
// <web>http://jsinh.in</web>
// <license>
// See license.txt in this project or http://http://jsinh.in/License-MIT.txt
// </license>
// ****************************************************************************

namespace DotNetStuffs.SimpleEvents
{
    /// <summary>
    /// This interface is meant for the <see cref="WeakAction{T}" /> class and can be 
    /// useful if you store multiple WeakAction{T} instances but don't know in advance
    /// what type T represents.
    /// </summary>
    public interface IExecuteWithObject
    {
        /// <summary>
        /// Gets the target of the WeakAction.
        /// </summary>
        object Target
        {
            get;
        }

        /// <summary>
        /// Executes an action.
        /// </summary>
        /// <param name="parameter">A parameter passed as an object,
        /// to be casted to the appropriate type.</param>
        void ExecuteWithObject(object parameter);

        /// <summary>
        /// Deletes all references, which notifies the cleanup method
        /// that this entry must be deleted.
        /// </summary>
        void MarkForDeletion();
    }
}