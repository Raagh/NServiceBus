﻿namespace NServiceBus.TransportTests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Transport;

    public class When_stopping : NServiceBusTransportTest
    {
        [TestCase(TransportTransactionMode.None)]
        [TestCase(TransportTransactionMode.ReceiveOnly)]
        [TestCase(TransportTransactionMode.SendsAtomicWithReceive)]
        [TestCase(TransportTransactionMode.TransactionScope)]
        public async Task Should_allow_message_processing_to_complete(TransportTransactionMode transactionMode)
        {
            var recoverabilityInvoked = false;

            var messageProcessingStarted = new TaskCompletionSource<bool>();
            var pumpStopping = new TaskCompletionSource<bool>();
            var completed = new TaskCompletionSource<CompleteContext>();

            OnTestTimeout(() =>
            {
                messageProcessingStarted.SetCanceled();
                completed.SetCanceled();
            });

            await StartPump(
                async (_, cancellationToken) =>
                {
                    messageProcessingStarted.SetResult(true);
                    await pumpStopping.Task;
                },
                (_, __) =>
                {
                    recoverabilityInvoked = true;
                    return Task.FromResult(ErrorHandleResult.Handled);
                },
                (context, _) =>
                {
                    completed.SetResult(context);
                    return Task.CompletedTask;
                },
                transactionMode);

            await SendMessage(InputQueueName);

            _ = await messageProcessingStarted.Task;

            var pumpTask = StopPump();
            pumpStopping.SetResult(true);

            await pumpTask;

            var completeContext = await completed.Task;

            Assert.False(recoverabilityInvoked, "Recoverability should not have been invoked.");
            Assert.True(completeContext.WasAcknowledged);
            Assert.IsFalse(completeContext.OnMessageFailed);
        }
    }
}
