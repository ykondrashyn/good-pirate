using System.Collections.Generic;
using GoodPirate113.Reconstruction.Modal;
using NUnit.Framework;

namespace GoodPirate113.Reconstruction.Tests
{
    public sealed class ModalCoordinatorTests
    {
        [Test]
        public void NestedLeasesOnlyPublishBoundaryTransitions()
        {
            var coordinator = new ModalCoordinator();
            var transitions = new List<bool>();
            coordinator.ModalStateChanged += transitions.Add;

            var first = coordinator.Acquire("inventory");
            var second = coordinator.Acquire("confirmation");

            Assert.That(coordinator.IsModalOpen, Is.True);
            Assert.That(coordinator.ActiveModalCount, Is.EqualTo(2));
            Assert.That(transitions, Is.EqualTo(new[] { true }));

            first.Dispose();
            Assert.That(coordinator.IsModalOpen, Is.True);
            Assert.That(transitions, Is.EqualTo(new[] { true }));

            second.Dispose();
            Assert.That(coordinator.IsModalOpen, Is.False);
            Assert.That(transitions, Is.EqualTo(new[] { true, false }));
        }

        [Test]
        public void LeaseDisposeIsIdempotent()
        {
            var coordinator = new ModalCoordinator();
            var lease = coordinator.Acquire("pause");

            lease.Dispose();
            lease.Dispose();

            Assert.That(coordinator.ActiveModalCount, Is.Zero);
            Assert.That(coordinator.IsModalOpen, Is.False);
        }
    }
}
