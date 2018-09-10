using System;

namespace StellarChecksum {
    /// <summary>
    /// Accounts are the central data structure in Stellar. Accounts are identified by a public Account Id and saved in the ledger.
    /// Everything else in the ledger, such as offers or trustlines, are owned by a particular account.
    /// </summary>
    public class StellarAccount {

        /// <summary>
        /// The public Account Id for this Stellar Account (immutable).
        /// </summary>
        public string AccountId { get; private set; }

        /// <summary>
        /// Hidden constructor to prevent construction of StellarAccounts with invalid checksums
        /// <param name="publicAccountId">The public Account Id</param>
        /// </summary>
        private StellarAccount(string publicAccountId) {
            AccountId = publicAccountId;
        }

        /// <summary>
        /// Use to create a new StellarAccount. This method is guarenteed to create a StellarAccount with a valid AccountID or
        /// it will throw a FormatException. This step is used to guarenteed validated data in the object.
        /// </summary>
        /// <param name="publicAccountId">The public Account Id</param>
        /// <returns>The StellarAccount to be created or NULL if the provided Account Id has an invalid checksum</returns>
        public static StellarAccount CreateAccount(string publicAccountId) {

            if (IsValid(publicAccountId)) {
                StellarAccount account = new StellarAccount(publicAccountId);

                return account;
            }
            return null;
        }

        /// <summary>
        /// This code calculates CRC16-XModem checksum
        /// Ported from https://github.com/alexgorbatchev/node-crc
        /// </summary>
        /// <param name="encoded">Stellar AccountId</param>
        /// <returns>true if this string has a valid CRC16-XModem checksum</returns>
        public static bool IsValid(string encoded) {

            try {
                byte[] decoded = ToBytes(encoded);
                byte[] payload = new byte[decoded.Length - 2];
                Array.Copy(decoded, payload, decoded.Length - 2);
                byte[] data = new byte[payload.Length - 1];
                Array.Copy(payload, 1, data, 0, payload.Length - 1);
                byte[] checksum = new byte[2];
                Array.Copy(decoded, decoded.Length - 2, checksum, 0, 2);

                byte[] expectedChecksum = CalculateChecksum(payload);

                if (expectedChecksum.Length != checksum.Length) {
                    return false;
                }

                for (int i = 0; i < checksum.Length; i++) {
                    if (expectedChecksum[i] != checksum[i]) {
                        return false;
                    }
                }
                return true;
            } catch (Exception) {
                return false;
            }
        }

        /// <summary>
        /// private helper
        /// </summary>
        /// <param name="bytes">the bytes</param>
        /// <returns>the bytes</returns>
        private static byte[] CalculateChecksum(byte[] bytes) {
            int crc = 0x0000;
            int count = bytes.Length;
            int i = 0;

            while (count > 0) {
                var code = (int)((uint)crc >> 8 & 0xFF);
                code ^= bytes[i++] & 0xFF;
                code ^= (int)((uint)code >> 4);
                crc = crc << 8 & 0xFFFF;
                crc ^= code;
                code = code << 5 & 0xFFFF;
                crc ^= code;
                code = code << 7 & 0xFFFF;
                crc ^= code;
                count--;
            }

            // little-endian
            return new[] { (byte)crc, (byte)((uint)crc >> 8) };
        }

        /// <summary>
        /// private helper function
        /// </summary>
        /// <param name="input">the input</param>
        /// <returns>the bytes</returns>
        private static byte[] ToBytes(string input) {
            if (string.IsNullOrEmpty(input)) {
                throw new ArgumentNullException("input");
            }

            input = input.TrimEnd('='); //remove padding characters
            int byteCount = input.Length * 5 / 8; //this must be TRUNCATED
            byte[] returnArray = new byte[byteCount];

            byte curByte = 0, bitsRemaining = 8;

            // ReSharper disable once TooWideLocalVariableScope
            int mask;

            int arrayIndex = 0;

            foreach (char c in input) {
                int cValue = CharToValue(c);

                if (bitsRemaining > 5) {
                    mask = cValue << (bitsRemaining - 5);
                    curByte = (byte)(curByte | mask);
                    bitsRemaining -= 5;
                } else {
                    mask = cValue >> (5 - bitsRemaining);
                    curByte = (byte)(curByte | mask);
                    returnArray[arrayIndex++] = curByte;
                    curByte = (byte)(cValue << (3 + bitsRemaining));
                    bitsRemaining += 3;
                }
            }

            //if we didn't end with a full byte
            if (arrayIndex != byteCount) {
                returnArray[arrayIndex] = curByte;
            }

            return returnArray;
        }

        /// <summary>
        /// private helper function
        /// </summary>
        /// <param name="c">char to be converted</param>
        /// <returns>integer value of char</returns>
        private static int CharToValue(char c) {
            int value = c;

            //65-90 == uppercase letters
            if (value < 91 && value > 64) {
                return value - 65;
            }
            //50-55 == numbers 2-7
            if (value < 56 && value > 49) {
                return value - 24;
            }
            //97-122 == lowercase letters
            if (value < 123 && value > 96) {
                return value - 97;
            }

            throw new ArgumentException("Character is not a Base32 character.", "c");
        }

        /// <summary>
        /// Compares equality of this and another object.
        /// </summary>
        /// <param name="obj">Object to compare equality to</param>
        /// <returns>True if the accounts are equal</returns>
        public override bool Equals(object obj) {
            StellarAccount account = obj as StellarAccount;
            if (account != null) {
                return AccountId.Equals(account.AccountId);
            }
            return false;
        }

        /// <summary>
        /// Returns a hash of this account
        /// </summary>
        /// <returns>Integer hash of this object</returns>
        public override int GetHashCode() {
            return AccountId.GetHashCode();
        }

        /// <summary>
        /// String repersentation of this account
        /// </summary>
        /// <returns>String repersentation of this account</returns>
        public override string ToString() {
            return AccountId;
        }
    }
}
