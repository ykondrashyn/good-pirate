using System;
using System.Collections.Generic;

namespace GoodPirate113.Reconstruction.Offline
{
    public enum OfflineProductKind
    {
        Permanent,
        OneTimePack,
        DailyPack,
        RepeatableCurrency
    }

    public readonly struct OfflineProductDefinition
    {
        public OfflineProductDefinition(string id, OfflineProductKind kind, int diamondAmount = 0)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Kind = kind;
            DiamondAmount = diamondAmount;
        }

        public string Id { get; }
        public OfflineProductKind Kind { get; }
        public int DiamondAmount { get; }
        public bool IsRestorable => Kind == OfflineProductKind.Permanent;
        public bool IsRepeatable => Kind == OfflineProductKind.RepeatableCurrency;
    }

    public static class GoodPirateOfflineProductCatalog
    {
        public const string RemoveAds = "goodpirate_removeads";
        public const string Parrot = "goodpirate_parrot";
        public const string Eagle = "goodpirate_eagle";
        public const string BlackBird = "goodpirate_blackbird";
        public const string RaftSkin = "goodpirate_skin_raft";
        public const string StarterPack = "goodpirate_pack_starter";
        public const string TodayPack = "goodpirate_pack_today";
        public const string BirdsPack = "goodpirate_pack_birds";
        public const string IntermediatePack = "goodpirate_pack_inter";
        public const string ExpertPack = "goodpirate_pack_expert";
        public const string Crystal00 = "goodpirate_crystal_00";
        public const string Crystal01 = "goodpirate_crystal_01";
        public const string Crystal02 = "goodpirate_crystal_02";
        public const string Crystal03 = "goodpirate_crystal_03";
        public const string Crystal04 = "goodpirate_crystal_04";
        public const string Crystal05 = "goodpirate_crystal_05";

        private static readonly OfflineProductDefinition[] Products =
        {
            new OfflineProductDefinition(RemoveAds, OfflineProductKind.Permanent),
            new OfflineProductDefinition(Parrot, OfflineProductKind.Permanent),
            new OfflineProductDefinition(Eagle, OfflineProductKind.Permanent),
            new OfflineProductDefinition(BlackBird, OfflineProductKind.Permanent),
            new OfflineProductDefinition(RaftSkin, OfflineProductKind.Permanent),
            new OfflineProductDefinition(StarterPack, OfflineProductKind.OneTimePack),
            new OfflineProductDefinition(TodayPack, OfflineProductKind.DailyPack),
            new OfflineProductDefinition(BirdsPack, OfflineProductKind.Permanent),
            new OfflineProductDefinition(IntermediatePack, OfflineProductKind.OneTimePack),
            new OfflineProductDefinition(ExpertPack, OfflineProductKind.OneTimePack),
            new OfflineProductDefinition(Crystal00, OfflineProductKind.RepeatableCurrency, 100),
            new OfflineProductDefinition(Crystal01, OfflineProductKind.RepeatableCurrency, 300),
            new OfflineProductDefinition(Crystal02, OfflineProductKind.RepeatableCurrency, 1000),
            new OfflineProductDefinition(Crystal03, OfflineProductKind.RepeatableCurrency, 2100),
            new OfflineProductDefinition(Crystal04, OfflineProductKind.RepeatableCurrency, 3900),
            new OfflineProductDefinition(Crystal05, OfflineProductKind.RepeatableCurrency, 7400)
        };

        public static IReadOnlyList<OfflineProductDefinition> All => Products;

        public static bool TryGet(string productId, out OfflineProductDefinition definition)
        {
            for (var index = 0; index < Products.Length; index++)
            {
                if (string.Equals(Products[index].Id, productId, StringComparison.Ordinal))
                {
                    definition = Products[index];
                    return true;
                }
            }

            definition = default;
            return false;
        }
    }
}
