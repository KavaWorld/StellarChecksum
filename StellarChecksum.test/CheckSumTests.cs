using NUnit.Framework;

namespace StellarChecksum.test {

    [TestFixture]
    public class CheckSumTests {

        [TestCase("GD4T35DMXYDE7BJWYPUWK43VFJO5IBUQYG2YGMICPTWP4JTNWQELKAVA", ExpectedResult = true)]
        [TestCase("GDMMOKAEIMPIZF3XI2VC75CYBIVNKEKW6WVPWGPMNQUNEVCVSOAHPWFD", ExpectedResult = true)]
        [TestCase("GBX6DXELQKLHMKVX2G24E3TPQV6APUAQECIC3XUJJ77Y2NYDM66TDTVY", ExpectedResult = true)]
        [TestCase("GBVEQCPQS7G4GGXSIMM2T4FFEKCR2II4HF3HEXMFJESC2TKB5IC4UX5C", ExpectedResult = true)]
        [TestCase("GANYOWWYZQMXGJHELUG2NJBPB3BEL2OMAQPCQQHKLVWRRPZEYSP6VT3A", ExpectedResult = true)]
        [TestCase("GBH75U7JSLPWYAVYEUUMFDX6L5FMKZZP6NXZ3PXBCFQXMM3XGRZTHI72", ExpectedResult = true)]
        [TestCase("GDRQMBUFMZ5KVMG22RFWMCZFT4CCX7UHNQQVV4ISEB4E5HXREYRY4KEY", ExpectedResult = true)]
        [TestCase("GA336RBL6RNDUNNIDHDDXPB4ASY2X7WB24XRPB7LGRBP4GDX7P35JIR6", ExpectedResult = true)]
        [TestCase("GASVF25626CSXZFWCFTS2SPIKMNAXNXXJEBQXT4OKZU5T5VRJV64TFTW", ExpectedResult = true)]
        [TestCase("GAEX2ZVP4KDTACV5NSPQYUTCRDC3GTLA6IXWAYW2HBPPK3F5CWVHO3U5", ExpectedResult = true)]
        [TestCase("0x6f46cf5569aefa1acc1009290c8e043747172d89", ExpectedResult = false)]
        [TestCase("0x90e63c3d53e0ea496845b7a03ec7548b70014a91", ExpectedResult = false)]
        [TestCase("null", ExpectedResult = false)]
        [TestCase("not valid", ExpectedResult = false)]
        [TestCase(null, ExpectedResult = false)]
        [TestCase("", ExpectedResult = false)]
        [Author("Kava.World")]
        public bool TestMethod(string accountId)
        {
            return StellarAccount.IsValid(accountId);
        }
    }
}
