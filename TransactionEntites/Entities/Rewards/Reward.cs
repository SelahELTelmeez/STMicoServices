﻿using TransactionEntites.Entities.Shared;

namespace TransactionEntites.Entities.Rewards
{
    public class Reward : TrackableEntity
    {
        public Guid StudentIdentityId { get; set; }
        public int Type { get; set; }

        public int Term { get; set; }
        public int Grade { get; set; }
        public string SubjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsNew { get; set; }
        public MedalLevel MedalLevel { get; set; }
        public string Image { get; set; }
    }
}
