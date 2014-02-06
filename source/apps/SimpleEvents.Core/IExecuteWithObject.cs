// **************************************************************************
// <copyright file="IExecuteWithObject.cs" company="DotNetStuffs.in">
// Copyright © DotNetStuffs.in 2014
// </copyright>
// ****************************************************************************
// <author>Jaspalsinh Chauhan</author>
// <email>jachauhan@gmail.com</email>
// <date>25.01.2014</date>
// <project>SimpleEvents</project>
// <web>http://simpleevents.in</web>
// <license>
// Visit http://simpleevents.in/license for more license details.
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
        /// <param name="parameter">A parameter passed as an object, to be casted to the appropriate type.</param>
        void ExecuteWithObject(object parameter);

        /// <summary>
        /// Deletes all references, which notifies the cleanup method that this entry must be deleted.
        /// </summary>
        void MarkForDeletion();
    }
}