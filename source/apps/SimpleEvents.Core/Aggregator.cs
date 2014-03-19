// ************************************************************************************
// <copyright file="Aggregator.cs" company="GalaSoft Laurent Bugnion">
// Copyright © GalaSoft Laurent Bugnion 2009-2014
// </copyright>
// ************************************************************************************
// <author>Laurent Bugnion</author>
// <email>laurent@galasoft.ch</email>
// <date>13.4.2009</date>
// <project>GalaSoft.MvvmLight.Messaging</project>
// <web>http://www.galasoft.ch</web>
// <license>
// The work is originally forked / taken from GalaSoft.MvvmLight toolkit codebase 
// which is licensed under MIT license and remains copyright work of Laurent Bugnion.
// See license.txt in this project or visit http://www.galasoft.ch/license_MIT.txt
//
// Note: This entity and members within were renamed which was originally named 
// as "Messenger" in author's original work.
// </license>
// ************************************************************************************

namespace DotNetStuffs.SimpleEvents
{
    #region Namespace

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows.Threading;

    #endregion

    /// <summary>
    /// Aggregator is a class allowing objects to exchange notification.
    /// </summary>
    public class Aggregator : IAggregator
    {
        #region Variable declaration

        /// <summary>
        /// Aggregator creation lock instance.
        /// </summary>
        private static readonly object CreationLock = new object();

        /// <summary>
        /// Default instance of event aggregator.
        /// </summary>
        private static IAggregator defaultInstance;

        /// <summary>
        /// Aggregator subscription lock instance.
        /// </summary>
        private readonly object subscribeLock = new object();

        /// <summary>
        /// Sub classes action subscribers.
        /// </summary>
        private Dictionary<Type, List<WeakActionAndToken>> subscribersOfSubclassesAction;

        /// <summary>
        /// Strict action subscribers.
        /// </summary>
        private Dictionary<Type, List<WeakActionAndToken>> subscribersStrictAction;

        /// <summary>
        /// Indicates a value whether clean-up subscription or not.
        /// </summary>
        private bool isCleanupRegistered;

        #endregion

        #region Static public methods

        /// <summary>
        /// Gets the Aggregator's default instance, allowing to subscribe and send notification in a static manner.
        /// </summary>
        public static IAggregator Default
        {
            get
            {
                if (null != defaultInstance)
                {
                    return defaultInstance;
                }

                lock (CreationLock)
                {
                    defaultInstance = new Aggregator();
                }

                return defaultInstance;
            }
        }

        /// <summary>
        /// Provides a way to override the Aggregator.Default instance with
        /// a custom instance, for example for unit testing purposes.
        /// </summary>
        /// <param name="newAggregator">The instance that will be used as Aggregator.Default.</param>
        public static void OverrideDefault(IAggregator newAggregator)
        {
            defaultInstance = newAggregator;
        }

        /// <summary>
        /// Sets the Aggregator's default (static) instance to null.
        /// </summary>
        public static void Reset()
        {
            defaultInstance = null;
        }

        #endregion

        #region Subscribe implementation

        /// <summary>
        /// Subscribes a subscriber for a type of TNotification. The handler parameter will be executed when a corresponding notification is sent.
        /// <para>Subscribing a subscriber does not create a hard reference to it, so if this subscriber is deleted, no memory leak is caused.</para>
        /// </summary>
        /// <typeparam name="TNotification">The type of notification that the subscriber subscribes for.</typeparam>
        /// <param name="subscriber">Subscriber that will receive the notification.</param>
        /// <param name="handler">A handler that will be executed when a notification of type TNotification is sent.</param>
        public virtual void Subscribe<TNotification>(object subscriber, Action<TNotification> handler)
        {
            Subscribe(subscriber, handler, null, false);
        }

        /// <summary>
        /// Subscribes a subscriber for a type of TNotification. The handler parameter will be executed when a corresponding notification is sent.
        /// <para>Subscribing a subscriber does not create a hard reference to it, so if this subscriber is deleted, no memory leak is caused.</para>
        /// </summary>
        /// <typeparam name="TNotification">The type of notification that the subscriber subscribes for.</typeparam>
        /// <param name="subscriber">Subscriber that will receive the notifications.</param>
        /// <param name="handler">A handler that will be executed when a notification of type TNotification is sent.</param>
        /// <param name="receiveDerivedNotifications">If true, notification types deriving from TNotification will also be transmitted to the subscriber.
        /// For example, if a TypeAlphaNotification and an TypeBetaNotification derive from FinalNotification,
        /// subscribing for FinalNotification and setting receiveDerivedNotifications to true will send
        /// TypeAlphaNotification and TypeBetaNotification to the subscriber that subscribed.
        /// <para>Also, if TNotification is an interface, notification types implementing TNotification will also be
        /// transmitted to the subscriber.
        /// For example, if a TypeAlphaNotification and an TypeBetaNotification implement IFinalNotification, subscribing for IFinalNotification
        /// and setting receiveDerivedNotifications to true will send TypeAlphaNotification and TypeBetaNotification to the recipient that subscribed.</para>
        /// </param>
        public virtual void Subscribe<TNotification>(object subscriber, Action<TNotification> handler, bool receiveDerivedNotifications)
        {
            Subscribe(subscriber, handler, null, receiveDerivedNotifications);
        }

        /// <summary>
        /// Subscribes a subscriber for a type of TNotification. The handler parameter will be executed when a corresponding notification is sent.
        /// <para>Subscribing a subscriber does not create a hard reference to it, so if this subscriber is deleted, no memory leak is caused.</para>
        /// </summary>
        /// <typeparam name="TNotification">The type of notification that the subscriber subscribes for.</typeparam>
        /// <param name="subscriber">Subscriber that will receive the notification.</param>
        /// <param name="handler">A handler that will be executed when a notification of type TNotification is sent.</param>
        /// <param name="token">A token for a notification channel. If a subscriber subscribes using a token,
        /// and a publisher publishes a notification using the same token, then this notification will be
        /// delivered to the subscriber. Other subscribers who did not use a token when subscribing
        /// (or who used a different token) will not get the notification.
        /// Similarly, notifications published without any token, or with a different token, will not be delivered to that subscriber.</param>
        public virtual void Subscribe<TNotification>(object subscriber, Action<TNotification> handler, object token)
        {
            Subscribe(subscriber, handler, token, false);
        }

        /// <summary>
        /// Subscribes a subscriber for a type of TNotification. The handler parameter will be executed when a corresponding notification is sent.
        /// <para>Subscribing a subscriber does not create a hard reference to it, so if this subscriber is deleted, no memory leak is caused.</para>
        /// </summary>
        /// <typeparam name="TNotification">The type of notification that the subscriber subscribes for.</typeparam>
        /// <param name="subscriber">Subscriber that will receive the notification.</param>
        /// <param name="handler">A handler that will be executed when a notification of type TNotification is sent.</param>
        /// <param name="token">A token for a notification channel. If a subscriber subscribes using a token,
        /// and a publisher publishes a notification using the same token, then this notification will be
        /// delivered to the subscriber. Other subscribers who did not use a token when subscribing
        /// (or who used a different token) will not get the notification.
        /// Similarly, notifications published without any token, or with a different token, will not be delivered to that subscriber.</param>
        /// <param name="receiveDerivedNotifications">If true, notification types deriving from TNotification will also be transmitted to the subscriber.
        /// For example, if a TypeAlphaNotification and an TypeBetaNotification derive from FinalNotification,
        /// subscribing for FinalNotification and setting receiveDerivedNotifications to true will send
        /// TypeAlphaNotification and TypeBetaNotification to the subscriber that subscribed.
        /// <para>Also, if TNotification is an interface, notification types implementing TNotification will also be
        /// transmitted to the subscriber.
        /// For example, if a TypeAlphaNotification and an TypeBetaNotification implement IFinalNotification, subscribing for IFinalNotification
        /// and setting receiveDerivedNotifications to true will send TypeAlphaNotification and TypeBetaNotification to the recipient that subscribed.</para>
        /// </param>
        public virtual void Subscribe<TNotification>(object subscriber, Action<TNotification> handler, object token, bool receiveDerivedNotifications)
        {
            lock (this.subscribeLock)
            {
                var notificationType = typeof(TNotification);
                Dictionary<Type, List<WeakActionAndToken>> subscribers;
                if (receiveDerivedNotifications)
                {
                    if (null == this.subscribersOfSubclassesAction)
                    {
                        this.subscribersOfSubclassesAction = new Dictionary<Type, List<WeakActionAndToken>>();
                    }

                    subscribers = this.subscribersOfSubclassesAction;
                }
                else
                {
                    if (null == this.subscribersStrictAction)
                    {
                        this.subscribersStrictAction = new Dictionary<Type, List<WeakActionAndToken>>();
                    }

                    subscribers = this.subscribersStrictAction;
                }

                lock (subscribers)
                {
                    List<WeakActionAndToken> weakActionAndTokens;
                    if (!subscribers.ContainsKey(notificationType))
                    {
                        weakActionAndTokens = new List<WeakActionAndToken>();
                        subscribers.Add(notificationType, weakActionAndTokens);
                    }
                    else
                    {
                        weakActionAndTokens = subscribers[notificationType];
                    }

                    var weakAction = new WeakAction<TNotification>(subscriber, handler);

                    var item = new WeakActionAndToken
                    {
                        Action = weakAction,
                        Token = token
                    };

                    weakActionAndTokens.Add(item);
                }
            }

            this.RequestCleanup();
        }

        #endregion

        #region Publish implementation

        /// <summary>
        /// Publishes a notification to subscribers. The notification will reach all subscribers that subscribed for this notification type using one of the Subscribe methods.
        /// </summary>
        /// <typeparam name="TNotification">The type of notification that will be published.</typeparam>
        /// <param name="notification">The notification published to subscribers.</param>
        public virtual void Publish<TNotification>(TNotification notification)
        {
            PublishToTargetOrType(notification, null, null);
        }

        /// <summary>
        /// Publishes a notification to subscribers. The notification will reach all subscribers that subscribed for this notification type using one of the Subscribe methods, and that are of the targetType.
        /// </summary>
        /// <typeparam name="TNotification">The type of notification that will be published.</typeparam>
        /// <typeparam name="TTarget">The type of subscribers that will receive the notification. Notification won't be published to subscribers of another type.</typeparam>
        /// <param name="notification">The notification published to subscribers.</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Better usability, suppression is OK here !!")]
        public virtual void Publish<TNotification, TTarget>(TNotification notification)
        {
            PublishToTargetOrType(notification, typeof(TTarget), null);
        }

        /// <summary>
        /// Publishes a notification to subscribers. The notification will reach all subscribers that subscribed for this notification type using one of the Subscribe methods.
        /// </summary>
        /// <typeparam name="TNotification">The type of notification that will be published.</typeparam>
        /// <param name="notification">The notification published to subscribers.</param>
        /// <param name="token">A token for a notification channel. If a subscriber subscribers using a token,
        /// and a publisher publishes a notification using the same token, then this notification will be
        /// delivered to the subscriber. Other subscribers who did not use a token when subscribing
        /// (or who used a different token) will not get the notification.
        /// Similarly, notifications published without any token, or with a different token, will not be delivered to that subscriber.</param>
        public virtual void Publish<TNotification>(TNotification notification, object token)
        {
            PublishToTargetOrType(notification, null, token);
        }
        
        #endregion

        #region Unsubscribe implementation

        /// <summary>
        /// Unsubscribes a notification subscriber completely. After this method is executed, the subscriber will not receive any notification anymore.
        /// </summary>
        /// <param name="subscriber">The subscriber that must be un-subscribed.</param>
        public virtual void Unsubscribe(object subscriber)
        {
            UnsubscribeFromLists(subscriber, this.subscribersOfSubclassesAction);
            UnsubscribeFromLists(subscriber, this.subscribersStrictAction);
        }

        /// <summary>
        /// Unsubscribes a subscriber for a given type of notifications only.
        /// After this method is executed, the subscriber will not receive notifications of 
        /// type TNotification anymore, but will still receive other notification types
        /// (if it subscribed for them previously).
        /// </summary>
        /// <param name="subscriber">The subscriber that must be un-subscribed.</param>
        /// <typeparam name="TNotification">The type of notification that the subscriber wants to unsubscribe from.</typeparam>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Better usability, suppression is OK here !!")]
        public virtual void Unsubscribe<TNotification>(object subscriber)
        {
            this.Unsubscribe<TNotification>(subscriber, null, null);
        }

        /// <summary>
        /// Unsubscribes a subscriber for a given type of notifications only and for a given token.
        /// After this method is executed, the subscriber will not receive notifications of
        /// type TNotification anymore with the given token, but will still receive other 
        /// notification types or notifications with other tokens (if it subscribed for them previously).
        /// </summary>
        /// <param name="subscriber">The subscriber that must be un-subscribed.</param>
        /// <param name="token">The token for which the subscriber must be unsubscribed.</param>
        /// <typeparam name="TNotification">The type of notification that the subscriber wants to unsubscribe from.</typeparam>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Better usability, suppression is OK here !!")]
        public virtual void Unsubscribe<TNotification>(object subscriber, object token)
        {
            this.Unsubscribe<TNotification>(subscriber, token, null);
        }

        /// <summary>
        /// Unsubscribes a subscriber for a given type of notifications only and for a given handler.
        /// Other notification types will still be transmitted to the subscriber (if it subscribed for them previously).
        /// Other handlers that have been subscribed for the notification type TNotification and for the given subscriber (if available) will also remain available.
        /// </summary>
        /// <typeparam name="TNotification">The type of notification that the subscriber wants to unsubscribe from.</typeparam>
        /// <param name="subscriber">The subscriber that must be un-subscribed.</param>
        /// <param name="handler">Handler that must be unsubscribed for the subscriber and for the notification type TNotification.</param>
        public virtual void Unsubscribe<TNotification>(object subscriber, Action<TNotification> handler)
        {
            this.Unsubscribe(subscriber, null, handler);
        }

        /// <summary>
        /// Unsubscribes a subscriber for a given type of notifications, for a given handler and a given token.
        /// Other notification types will still be transmitted to the subscriber (if it subscribed for them previously).
        /// Other handlers that have been subscribed for the notification type TNotification, for the given subscriber and other tokens (if available) will also remain available.
        /// </summary>
        /// <typeparam name="TNotification">The type of notification that the subscriber wants to unsubscribe from.</typeparam>
        /// <param name="subscriber">The subscriber that must be un-subscribed.</param>
        /// <param name="token">The token for which the subscriber must be unsubscribed.</param>
        /// <param name="handler">Handler that must be unsubscribed for the subscriber and for the notification type TNotification.</param>
        public virtual void Unsubscribe<TNotification>(object subscriber, object token, Action<TNotification> handler)
        {
            UnsubscribeFromLists(subscriber, token, handler, this.subscribersStrictAction);
            UnsubscribeFromLists(subscriber, token, handler, this.subscribersOfSubclassesAction);
            this.RequestCleanup();
        }
        
        #endregion

        #region Cleanup methods

        /// <summary>
        /// Provides a non-static access to the static <see cref="Reset"/> method.
        /// Sets the Aggregator's default (static) instance to null.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Static access not required, suppression is OK here !!")]
        public void ResetAll()
        {
            Reset();
        }

        /// <summary>
        /// Notifies the Aggregator that the lists of subscribers should be scanned and cleaned up.
        /// Since subscribers are stored as <see cref="WeakReference"/>, subscribers can be 
        /// garbage collected even though the Aggregator keeps them in a list.
        /// During the cleanup operation, all "dead" subscribers are removed from the lists.
        /// Since this operation can take a moment, it is only executed when the application is idle.
        /// For this reason, a user of the Aggregator class should use <see cref="RequestCleanup"/>
        /// instead of forcing one with the  <see cref="Cleanup" /> method.
        /// </summary>
        public void RequestCleanup()
        {
            if (this.isCleanupRegistered)
            {
                return;
            }

            Action cleanupAction = this.Cleanup;
            Dispatcher.CurrentDispatcher.BeginInvoke(cleanupAction, DispatcherPriority.ApplicationIdle, null);
            this.isCleanupRegistered = true;
        }

        /// <summary>
        /// Scans the subscriber's lists for "dead" instances and removes them.
        /// Since subscribers are stored as <see cref="WeakReference"/>, subscribers can be
        /// garbage collected even though the Aggregator keeps them in a list.
        /// During the cleanup operation, all "dead" subscribers are removed from the lists.
        /// Since this operation can take a moment, it is only executed when the application is idle.
        /// For this reason, a user of the Aggregator class should use <see cref="RequestCleanup"/>
        /// instead of forcing one with the  <see cref="Cleanup" /> method.
        /// </summary>
        public void Cleanup()
        {
            CleanupList(this.subscribersOfSubclassesAction);
            CleanupList(this.subscribersStrictAction);
            this.isCleanupRegistered = false;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Clean-up list of type - weak action and token.
        /// </summary>
        /// <param name="weakActionAndTokens">Dictionary list of type - weak action and token.</param>
        private static void CleanupList(IDictionary<Type, List<WeakActionAndToken>> weakActionAndTokens)
        {
            if (null == weakActionAndTokens)
            {
                return;
            }

            lock (weakActionAndTokens)
            {
                var listsToRemove = new List<Type>();
                foreach (var weakActionAndToken in weakActionAndTokens)
                {
                    var recipientsToRemove = weakActionAndToken.Value.Where(item => item.Action == null || !item.Action.IsAlive).ToList();
                    foreach (var recipient in recipientsToRemove)
                    {
                        weakActionAndToken.Value.Remove(recipient);
                    }

                    if (0 == weakActionAndToken.Value.Count)
                    {
                        listsToRemove.Add(weakActionAndToken.Key);
                    }
                }

                foreach (var key in listsToRemove)
                {
                    weakActionAndTokens.Remove(key);
                }
            }
        }

        /// <summary>
        /// List of weak action and token instance to send notification.
        /// </summary>
        /// <typeparam name="TNotification">Type of notification to publish.</typeparam>
        /// <param name="notification">Notification to publish.</param>
        /// <param name="weakActionsAndTokens">List of weak action and token</param>
        /// <param name="notificationTargetType">Notification target type.</param>
        /// <param name="token">Notification subscription token.</param>
        private static void SendToList<TNotification>(TNotification notification, IEnumerable<WeakActionAndToken> weakActionsAndTokens, Type notificationTargetType, object token)
        {
            if (null == weakActionsAndTokens)
            {
                return;
            }

            //// Clone to protect from user subscribing in a "receive notification" method
            var list = weakActionsAndTokens.ToList();
            var listClone = list.Take(list.Count()).ToList();
            foreach (var executeAction in
                from item in listClone
                let executeAction = item.Action as IExecuteWithObject
                where
                    executeAction != null && item.Action.IsAlive && item.Action.Target != null
                    && (
                           null == notificationTargetType
                           || item.Action.Target.GetType() == notificationTargetType
                           || notificationTargetType.IsInstanceOfType(item.Action.Target))
                    && ((null == item.Token && null == token) || (null != item.Token && item.Token.Equals(token)))
                select executeAction)
            {
                executeAction.ExecuteWithObject(notification);
            }
        }

        /// <summary>
        /// Unsubscribe subscriber from list.
        /// </summary>
        /// <param name="subscriber">Instance of subscriber.</param>
        /// <param name="lists">List of weak action and token.</param>
        private static void UnsubscribeFromLists(object subscriber, Dictionary<Type, List<WeakActionAndToken>> lists)
        {
            if (null == subscriber || null == lists || 0 == lists.Count)
            {
                return;
            }

            lock (lists)
            {
                foreach (var weakAction in lists.Keys.SelectMany(messageType => lists[messageType].Select(item => (IExecuteWithObject)item.Action).Where(weakAction => null != weakAction && subscriber == weakAction.Target)))
                {
                    weakAction.MarkForDeletion();
                }
            }
        }

        /// <summary>
        /// Unsubscribe from list.
        /// </summary>
        /// <typeparam name="TNotification">Type of notification.</typeparam> 
        /// <param name="subscriber">Instance of subscriber.</param>
        /// <param name="token">Notification subscription token.</param>
        /// <param name="handler">Instance of handler of type notification.</param>
        /// <param name="lists">List of weak action and token.</param>
        private static void UnsubscribeFromLists<TNotification>(object subscriber, object token, Action<TNotification> handler, Dictionary<Type, List<WeakActionAndToken>> lists)
        {
            var notificationType = typeof(TNotification);
            if (null == subscriber || null == lists || 0 == lists.Count || !lists.ContainsKey(notificationType))
            {
                return;
            }

            lock (lists)
            {
                foreach (var item in from item in lists[notificationType] let weakActionCasted = item.Action as WeakAction<TNotification> where null != weakActionCasted && subscriber == weakActionCasted.Target && (null == handler || handler.Method.Name == weakActionCasted.MethodName) && (null == token || token.Equals(item.Token)) select item)
                {
                    item.Action.MarkForDeletion();
                }
            }
        }

        /// <summary>
        /// Publish to target or type.
        /// </summary>
        /// <typeparam name="TNotification">Type of notification.</typeparam>
        /// <param name="notification">Notification to publish to target or type.</param>
        /// <param name="notificationTargetType">Notification target type.</param>
        /// <param name="token">Notification subscription token.</param>
        private void PublishToTargetOrType<TNotification>(TNotification notification, Type notificationTargetType, object token)
        {
            var notificationType = typeof(TNotification);
            if (null != this.subscribersOfSubclassesAction)
            {
                // Clone to protect from user subscribing in a "receive notification" method
                var listClone = this.subscribersOfSubclassesAction.Keys.Take(this.subscribersOfSubclassesAction.Count()).ToList();
                foreach (var type in listClone)
                {
                    List<WeakActionAndToken> list = null;
                    if (notificationType == type || notificationType.IsSubclassOf(type) || type.IsAssignableFrom(notificationType))
                    {
                        lock (this.subscribersOfSubclassesAction)
                        {
                            list = this.subscribersOfSubclassesAction[type].Take(this.subscribersOfSubclassesAction[type].Count()).ToList();
                        }
                    }

                    SendToList(notification, list, notificationTargetType, token);
                }
            }

            if (this.subscribersStrictAction != null)
            {
                List<WeakActionAndToken> list = null;
                lock (this.subscribersStrictAction)
                {
                    if (this.subscribersStrictAction.ContainsKey(notificationType))
                    {
                        list = this.subscribersStrictAction[notificationType].Take(this.subscribersStrictAction[notificationType].Count()).ToList();
                    }
                }

                if (list != null)
                {
                    SendToList(notification, list, notificationTargetType, token);
                }
            }

            this.RequestCleanup();
        }
        
        #endregion

        #region Action and Token structure

        /// <summary>
        /// Weak action and token structure.
        /// </summary>
        private struct WeakActionAndToken
        {
            /// <summary>
            /// Instance of weak action.
            /// </summary>
            public WeakAction Action;

            /// <summary>
            /// Instance of token used at the time of subscription.
            /// </summary>
            public object Token;
        }
        
        #endregion
    }
}