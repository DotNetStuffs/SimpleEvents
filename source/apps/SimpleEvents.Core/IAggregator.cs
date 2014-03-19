// ************************************************************************************
// <copyright file="IAggregator.cs" company="GalaSoft Laurent Bugnion">
// Copyright © GalaSoft Laurent Bugnion 2009-2014
// </copyright>
// ************************************************************************************
// <author>Laurent Bugnion</author>
// <email>laurent@galasoft.ch</email>
// <date>23.9.2009</date>
// <project>GalaSoft.MvvmLight.Messaging</project>
// <web>http://www.galasoft.ch</web>
// <license>
// The work is originally forked / taken from GalaSoft.MvvmLight toolkit codebase 
// which is licensed under MIT license and remains copyright work of Laurent Bugnion.
// See license.txt in this project or visit http://www.galasoft.ch/license_MIT.txt
//
// Note: This entity and members within were renamed which was originally named 
// as "IMessenger" in author's original work.
// </license>
// ************************************************************************************

namespace DotNetStuffs.SimpleEvents
{
    #region Namespace

    using System;
    using System.Diagnostics.CodeAnalysis;
    
    #endregion

    /// <summary>
    /// The Aggregator is a class allowing objects to notify each other.
    /// </summary>
    public interface IAggregator
    {
        #region Subscribe implementation

        /// <summary>
        /// Subscribes a subscriber for a type of TNotification. The handler parameter will be executed when a corresponding notification is sent.
        /// <para>Subscribing a subscriber does not create a hard reference to it, so if this subscriber is deleted, no memory leak is caused.</para>
        /// </summary>
        /// <typeparam name="TNotification">The type of notification that the subscriber subscribes for.</typeparam>
        /// <param name="subscriber">Subscriber that will receive the notification.</param>
        /// <param name="handler">A handler that will be executed when a notification of type TNotification is sent.</param>
        void Subscribe<TNotification>(object subscriber, Action<TNotification> handler);

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
        void Subscribe<TNotification>(object subscriber,Action<TNotification> handler,bool receiveDerivedNotifications);

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
        void Subscribe<TNotification>(object subscriber, Action<TNotification> handler, object token);

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
        void Subscribe<TNotification>(object subscriber,Action<TNotification> handler,object token,bool receiveDerivedNotifications);

        #endregion

        #region Publish implementation

        /// <summary>
        /// Publishes a notification to subscribers. The notification will reach all subscribers that subscribed for this notification type using one of the Subscribe methods.
        /// </summary>
        /// <typeparam name="TNotification">The type of notification that will be published.</typeparam>
        /// <param name="notification">The notification published to subscribers.</param>
        void Publish<TNotification>(TNotification notification);

        /// <summary>
        /// Publishes a notification to subscribers. The notification will reach all subscribers that subscribed for this notification type using one of the Subscribe methods, and that are of the targetType.
        /// </summary>
        /// <typeparam name="TNotification">The type of notification that will be published.</typeparam>
        /// <typeparam name="TTarget">The type of subscribers that will receive the notification. Notification won't be published to subscribers of another type.</typeparam>
        /// <param name="notification">The notification published to subscribers.</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Better usability, suppression is OK here !!")]
        void Publish<TNotification, TTarget>(TNotification notification);

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
        void Publish<TNotification>(TNotification notification, object token);

        #endregion

        #region Unsubscribe implementation

        /// <summary>
        /// Unsubscribes a notification subscriber completely. After this method is executed, the subscriber will not receive any notification anymore.
        /// </summary>
        /// <param name="subscriber">The subscriber that must be un-subscribed.</param>
        void Unsubscribe(object subscriber);

        /// <summary>
        /// Unsubscribes a subscriber for a given type of notifications only.
        /// After this method is executed, the subscriber will not receive notifications of 
        /// type TNotification anymore, but will still receive other notification types
        /// (if it subscribed for them previously).
        /// </summary>
        /// <param name="subscriber">The subscriber that must be un-subscribed.</param>
        /// <typeparam name="TNotification">The type of notification that the subscriber wants to unsubscribe from.</typeparam>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Better usability, suppression is OK here !!")]
        void Unsubscribe<TNotification>(object subscriber);

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
        void Unsubscribe<TNotification>(object subscriber, object token);

        /// <summary>
        /// Unsubscribes a subscriber for a given type of notifications only and for a given handler.
        /// Other notification types will still be transmitted to the subscriber (if it subscribed for them previously).
        /// Other handlers that have been subscribed for the notification type TNotification and for the given subscriber (if available) will also remain available.
        /// </summary>
        /// <typeparam name="TNotification">The type of notification that the subscriber wants to unsubscribe from.</typeparam>
        /// <param name="subscriber">The subscriber that must be un-subscribed.</param>
        /// <param name="handler">Handler that must be unsubscribed for the subscriber and for the notification type TNotification.</param>
        void Unsubscribe<TNotification>(object subscriber, Action<TNotification> handler);

        /// <summary>
        /// Unsubscribes a subscriber for a given type of notifications, for a given handler and a given token.
        /// Other notification types will still be transmitted to the subscriber (if it subscribed for them previously).
        /// Other handlers that have been subscribed for the notification type TNotification, for the given subscriber and other tokens (if available) will also remain available.
        /// </summary>
        /// <typeparam name="TNotification">The type of notification that the subscriber wants to unsubscribe from.</typeparam>
        /// <param name="subscriber">The subscriber that must be un-subscribed.</param>
        /// <param name="token">The token for which the subscriber must be unsubscribed.</param>
        /// <param name="handler">Handler that must be unsubscribed for the subscriber and for the notification type TNotification.</param>
        void Unsubscribe<TNotification>(object subscriber, object token, Action<TNotification> handler);

        #endregion
    }
}