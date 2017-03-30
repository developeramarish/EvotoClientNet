﻿using System;

namespace Models
{
    public class BlockchainDetails
    {
        public string Name { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string ChainString { get; set; }
        public int Port { get; set; }
        public string WalletId { get; set; }
        public int Blocks { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}