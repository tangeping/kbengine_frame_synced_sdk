using System;
using System.Collections.Generic;
using UnityEngine;

namespace KBEngine {

    /**
    * @brief A concrete class based on {@link SerializableDictionary} with 'byte' as key and 'FP' as value.
    * 
    * This is necessary because Unity's json engine doesn't work with generic types.
    **/
    [Serializable]
    public class SerializableDictionaryByteFP : SerializableDictionary<byte, FP> { }

    /**
    * @brief A concrete class based on {@link SerializableDictionary} with 'byte' as key and 'FPVector' as value.
    * 
    * This is necessary because Unity's json engine doesn't work with generic types.
    **/
    [Serializable]
    public class SerializableDictionaryByteFPVector : SerializableDictionary<byte, FPVector> { }

    /**
    * @brief A concrete class based on {@link SerializableDictionary} with 'byte' as key and 'FPVector2' as value.
    * 
    * This is necessary because Unity's json engine doesn't work with generic types.
    **/
    [Serializable]
    public class SerializableDictionaryByteFPVector2 : SerializableDictionary<byte, FPVector2> { }

    /**
     * @brief Provides information about a player's inputs.
     **/
    [Serializable]
	public class InputData : InputDataBase {

        /**
         * @brief Contains data about string values.
         **/
        [SerializeField]
        internal SerializableDictionaryByteString stringTable;

        /**
         * @brief Contains data about byte values.
         **/
        [SerializeField]
        internal SerializableDictionaryByteByte byteTable;

        /**
         * @brief Contains data about int values.
         **/
        [SerializeField]
        internal SerializableDictionaryByteInt intTable;

        /**
         * @brief Contains data about FP values.
         **/
        [SerializeField]
        internal SerializableDictionaryByteFP fpTable;

        /**
         * @brief Contains data about byte[] values.
         **/
        [SerializeField]
        internal SerializableDictionaryByteByteArray byteArrayTable;

        /**
         * @brief Contains data about FPVector values.
         **/
        [SerializeField]
        internal SerializableDictionaryByteFPVector FPVectorTable;

        /**
         * @brief Contains data about FPVector values.
         **/
        [SerializeField]
        internal SerializableDictionaryByteFPVector2 FPVectorTable2;

        /**
        * @brief Possible types of input data.
        **/
        private enum Types : byte { Byte = 0, String = 1, Integer = 2, FP = 3, FPVector = 4, FPVector2 = 5, ByteArray = 6 };

        public InputData() {
            this.stringTable = new SerializableDictionaryByteString ();
			this.byteTable = new SerializableDictionaryByteByte ();
			this.intTable = new SerializableDictionaryByteInt ();
            this.fpTable = new SerializableDictionaryByteFP();
            this.byteArrayTable = new SerializableDictionaryByteByteArray();
            this.FPVectorTable = new SerializableDictionaryByteFPVector();
            this.FPVectorTable2 = new SerializableDictionaryByteFPVector2();
        }

        public override void Serialize(List<byte> bytes) {
            byte numberOfActions = (byte)(Count);

            bytes.Add(numberOfActions);

            var byteTableEnum = byteTable.GetEnumerator();
            while (byteTableEnum.MoveNext()) {
                KeyValuePair<byte, byte> pair = byteTableEnum.Current;

                bytes.Add(pair.Key);
                bytes.Add((byte) Types.Byte);
                bytes.Add(pair.Value);
            }

            var byteArrayTableEnum = byteArrayTable.GetEnumerator();
            while (byteArrayTableEnum.MoveNext()) {
                KeyValuePair<byte, byte[]> pair = byteArrayTableEnum.Current;

                bytes.Add(pair.Key);
                bytes.Add((byte)Types.ByteArray);
                Utils.GetBytes(pair.Value.Length, bytes);

                for (int index = 0, length = pair.Value.Length; index < length; index++) {
                    bytes.Add(pair.Value[index]);
                }
            }

            var intTableEnum = intTable.GetEnumerator();
            while (intTableEnum.MoveNext()) {
                KeyValuePair<byte, int> pair = intTableEnum.Current;

                bytes.Add(pair.Key);
                bytes.Add((byte)Types.Integer);
                Utils.GetBytes(pair.Value, bytes);
            }

            var stringTableEnum = stringTable.GetEnumerator();
            while (stringTableEnum.MoveNext()) {
                KeyValuePair<byte, string> pair = stringTableEnum.Current;
                bytes.Add(pair.Key);
                bytes.Add((byte)Types.String);
                byte[] strAscii = System.Text.Encoding.ASCII.GetBytes(pair.Value);
                Utils.GetBytes(strAscii.Length, bytes);

                for (int index = 0, length = strAscii.Length; index < length; index++) {
                    bytes.Add(strAscii[index]);
                }
            }

            var fpTableEnum = fpTable.GetEnumerator();
            while (fpTableEnum.MoveNext()) {
                KeyValuePair<byte, FP> pair = fpTableEnum.Current;

                bytes.Add(pair.Key);
                bytes.Add((byte)Types.FP);
                Utils.GetBytes(pair.Value.RawValue, bytes);
            }

            var FPVectorTableEnum = FPVectorTable.GetEnumerator();
            while (FPVectorTableEnum.MoveNext()) {
                KeyValuePair<byte, FPVector> pair = FPVectorTableEnum.Current;

                bytes.Add(pair.Key);
                bytes.Add((byte)Types.FPVector);
                Utils.GetBytes(pair.Value.x.RawValue, bytes);
                Utils.GetBytes(pair.Value.y.RawValue, bytes);
                Utils.GetBytes(pair.Value.z.RawValue, bytes);
            }

            var FPVector2TableEnum = FPVectorTable2.GetEnumerator();
            while (FPVector2TableEnum.MoveNext()) {
                KeyValuePair<byte, FPVector2> pair = FPVector2TableEnum.Current;

                bytes.Add(pair.Key);
                bytes.Add((byte)Types.FPVector2);
                Utils.GetBytes(pair.Value.x.RawValue, bytes);
                Utils.GetBytes(pair.Value.y.RawValue, bytes);
            }
        }

        public override void Deserialize(byte[] data, ref int offset) {
            byte numberOfActions = data[offset++];

            for (int i = 0; i < numberOfActions; i++) {
                byte key = data[offset++];
                byte type = data[offset++];

                switch (type) {
                    case (byte)Types.Integer:
                        int intValue = BitConverter.ToInt32(data, offset);
                        AddInt(key, intValue);
                        offset += sizeof(int);
                        break;

                    case (byte)Types.Byte:
                        byte byteValue = data[offset++];
                        AddByte(key, byteValue);
                        break;

                    case (byte)Types.ByteArray:
                        int byteArrayLen = BitConverter.ToInt32(data, offset);
                        offset += sizeof(int);

                        byte[] bArray = new byte[byteArrayLen];
                        for (int indexArray = 0; indexArray < byteArrayLen; indexArray++) {
                            bArray[indexArray] = data[offset + indexArray];
                        }

                        offset += byteArrayLen;

                        AddByteArray(key, bArray);
                        break;

                    case (byte)Types.String:
                        int strlen = BitConverter.ToInt32(data, offset);
                        offset += sizeof(int);
                        string stringValue = System.Text.Encoding.ASCII.GetString(data, offset, strlen);
                        offset += strlen;
                        AddString(key, stringValue);
                        break;

                    case (byte)Types.FP:
                        FP fpValue = FP.FromRaw(BitConverter.ToInt64(data, offset));
                        AddFP(key, fpValue);
                        offset += sizeof(long);
                        break;

                    case (byte)Types.FPVector:
                        FP fpValueX = FP.FromRaw(BitConverter.ToInt64(data, offset));
                        offset += sizeof(long);

                        FP fpValueY = FP.FromRaw(BitConverter.ToInt64(data, offset));
                        offset += sizeof(long);

                        FP fpValueZ = FP.FromRaw(BitConverter.ToInt64(data, offset));
                        offset += sizeof(long);

                        AddFPVector(key, new FPVector(fpValueX, fpValueY, fpValueZ));
                        break;

                    case (byte)Types.FPVector2:
                        FP fpValueX2 = FP.FromRaw(BitConverter.ToInt64(data, offset));
                        offset += sizeof(long);

                        FP fpValueY2 = FP.FromRaw(BitConverter.ToInt64(data, offset));
                        offset += sizeof(long);

                        AddFPVector2(key, new FPVector2(fpValueX2, fpValueY2));
                        break;

                    default:
                        //Not Implemented
                        break;
                }
            }
        }

        /**
         * @brief Deserialize from FS_ENTITY_DATA.
         **/

        public override void Deserialize(FS_ENTITY_DATA e)
        {
            if(e.entityid <=0)
            {
                return;
            }

            ownerID = e.entityid;

            int offset = 0;

            Deserialize(e.datas, ref offset);

        }
        /**
         * @brief Serialize data to FS_ENTITY_DATA.
         **/
        public override FS_ENTITY_DATA Serialize()
        {
            List<byte> data = new List<byte>();
            Serialize(data);
            FS_ENTITY_DATA e = new FS_ENTITY_DATA();
            e.entityid = ownerID;
            e.datas = data.ToArray();
            return e;
        }

       public override void CleanUp() {
            this.stringTable.Clear();
            this.byteTable.Clear();
            this.intTable.Clear();
            this.fpTable.Clear();
            this.byteArrayTable.Clear();
            this.FPVectorTable.Clear();
            this.FPVectorTable2.Clear();
        }

        public override void CopyFrom(InputDataBase fromBase) {
            InputData from = (InputData) fromBase;

            var stringTableEnum = from.stringTable.GetEnumerator();
            while (stringTableEnum.MoveNext()) {
                var kv = stringTableEnum.Current;
                this.stringTable.Add(kv.Key, kv.Value);
            }

            var byteTableEnum = from.byteTable.GetEnumerator();
            while (byteTableEnum.MoveNext()) {
                var kv = byteTableEnum.Current;
                this.byteTable.Add(kv.Key, kv.Value);
            }

            var intTableEnum = from.intTable.GetEnumerator();
            while (intTableEnum.MoveNext()) {
                var kv = intTableEnum.Current;
                this.intTable.Add(kv.Key, kv.Value);
            }

            var fpTableEnum = from.fpTable.GetEnumerator();
            while (fpTableEnum.MoveNext()) {
                var kv = fpTableEnum.Current;
                this.fpTable.Add(kv.Key, kv.Value);
            }

            var byteArrayTableEnum = from.byteArrayTable.GetEnumerator();
            while (byteArrayTableEnum.MoveNext()) {
                var kv = byteArrayTableEnum.Current;
                this.byteArrayTable.Add(kv.Key, kv.Value);
            }

            var FPVectorTableEnum = from.FPVectorTable.GetEnumerator();
            while (FPVectorTableEnum.MoveNext()) {
                var kv = FPVectorTableEnum.Current;
                this.FPVectorTable.Add(kv.Key, kv.Value);
            }

            var FPVectorTable2Enum = from.FPVectorTable2.GetEnumerator();
            while (FPVectorTable2Enum.MoveNext()) {
                var kv = FPVectorTable2Enum.Current;
                this.FPVectorTable2.Add(kv.Key, kv.Value);
            }
        }

        /**
        * @brief Returns true if this {@link SyncedData} has all actions information equals to the provided one.
        **/
        public override bool EqualsData(InputDataBase otherBase) {
            InputData other = (InputData) otherBase;

            if (this.stringTable.Count != other.stringTable.Count ||
                this.byteTable.Count != other.byteTable.Count ||
                this.intTable.Count != other.intTable.Count ||
                this.fpTable.Count != other.fpTable.Count ||
                this.byteArrayTable.Count != other.byteArrayTable.Count ||
                this.FPVectorTable.Count != other.FPVectorTable.Count ||
                this.FPVectorTable2.Count != other.FPVectorTable2.Count) {

                return false;
            }

            if (!checkEqualsTable(this, other)) {
                return false;
            }

            return true;
        }

        /**
        * @brief Returns true if all information in two {@link InputData} are equals.
        **/
        private static bool checkEqualsTable(InputData id1, InputData id2) {
            var stringTableEnum = id1.stringTable.GetEnumerator();

            while (stringTableEnum.MoveNext()) {
                var pair = stringTableEnum.Current;

                if (!id2.stringTable.ContainsKey(pair.Key) || pair.Value != id2.stringTable[pair.Key]) {
                    return false;
                }
            }

            var byteTableEnum = id1.byteTable.GetEnumerator();

            while (byteTableEnum.MoveNext()) {
                var pair = byteTableEnum.Current;

                if (!id2.byteTable.ContainsKey(pair.Key) || pair.Value != id2.byteTable[pair.Key]) {
                    return false;
                }
            }

            var intTableEnum = id1.intTable.GetEnumerator();

            while (intTableEnum.MoveNext()) {
                var pair = intTableEnum.Current;

                if (!id2.intTable.ContainsKey(pair.Key) || pair.Value != id2.intTable[pair.Key]) {
                    return false;
                }
            }

            var fpTableEnum = id1.fpTable.GetEnumerator();

            while (fpTableEnum.MoveNext()) {
                var pair = fpTableEnum.Current;

                if (!id2.fpTable.ContainsKey(pair.Key) || pair.Value != id2.fpTable[pair.Key]) {
                    return false;
                }
            }

            var byteArrayTableEnum = id1.byteArrayTable.GetEnumerator();

            while (byteArrayTableEnum.MoveNext()) {
                var pair = byteArrayTableEnum.Current;

                if (!id2.byteArrayTable.ContainsKey(pair.Key) || pair.Value != id2.byteArrayTable[pair.Key]) {
                    return false;
                }
            }

            var FPVectorTableEnum = id1.FPVectorTable.GetEnumerator();

            while (FPVectorTableEnum.MoveNext()) {
                var pair = FPVectorTableEnum.Current;

                if (!id2.FPVectorTable.ContainsKey(pair.Key) || pair.Value != id2.FPVectorTable[pair.Key]) {
                    return false;
                }
            }

            var FPVectorTable2Enum = id1.FPVectorTable2.GetEnumerator();

            while (FPVectorTable2Enum.MoveNext()) {
                var pair = FPVectorTable2Enum.Current;

                if (!id2.FPVectorTable2.ContainsKey(pair.Key) || pair.Value != id2.FPVectorTable2[pair.Key]) {
                    return false;
                }
            }

            return true;
        }


        /**
         * @brief Returns how many key were added.
         **/
        public int Count {
            get {
                return (this.stringTable.Count + this.byteTable.Count + this.intTable.Count + this.fpTable.Count + this.byteArrayTable.Count + this.FPVectorTable.Count + this.FPVectorTable2.Count);
            }
        }

        /**
         * @brief Returns true if there is no input information.
         **/
        internal bool IsEmpty() {
			return Count == 0;
		}

        /**
         * @brief Adds a new string value.
         **/
        internal void AddString(byte key, string value){
			this.stringTable[key] = value;
		}

        /**
         * @brief Adds a new byte value.
         **/
        internal void AddByte(byte key, byte value){
			this.byteTable[key] = value;
		}

        /**
         * @brief Adds a new byte[] value.
         **/
        internal void AddByteArray(byte key, byte[] value) {
            byte[] newValue = new byte[value.Length];
            for (int index = 0, length = newValue.Length; index < length;  index++) {
                newValue[index] = value[index];
            }

            this.byteArrayTable[key] = newValue;
        }

        /**
         * @brief Adds a new int value.
         **/
        internal void AddInt(byte key, int value){
			this.intTable[key] = value;
		}

        /**
         * @brief Adds a new FP value.
         **/
        internal void AddFP(byte key, FP value) {
            this.fpTable[key] = value;
        }

        /**
         * @brief Adds a new FPVector value.
         **/
        internal void AddFPVector(byte key, FPVector value) {
            this.FPVectorTable[key] = value;
        }

        /**
         * @brief Adds a new FPVector2 value.
         **/
        internal void AddFPVector2(byte key, FPVector2 value) {
            this.FPVectorTable2[key] = value;
        }

        /**
         * @brief Gets a string value.
         **/
        public string GetString(byte key) {
			if (!this.stringTable.ContainsKey(key)) {
				return "";
			}

            return  this.stringTable[key];
        }

        /**
         * @brief Gets a byte value.
         **/
        public byte GetByte(byte key) {
			if (!this.byteTable.ContainsKey(key)) {
				return 0;
			}

            return this.byteTable[key];
        }

        /**
         * @brief Gets a byte[] value.
         **/
        public byte[] GetByteArray(byte key) {
            if (!this.byteArrayTable.ContainsKey(key)) {
                return null;
            }

            return this.byteArrayTable[key];
        }

        /**
         * @brief Gets a int value.
         **/
        public int GetInt(byte key) {
            if (!this.intTable.ContainsKey(key)) {
                return 0;
            }

            return this.intTable[key];
        }

        /**
         * @brief Gets a FP value.
         **/
        public FP GetFP(byte key) {
            if (!this.fpTable.ContainsKey(key)) {
                return 0;
            }

            return this.fpTable[key];
        }

        /**
         * @brief Gets a FPVector value.
         **/
        public FPVector GetFPVector(byte key) {
            if (!this.FPVectorTable.ContainsKey(key)) {
                return FPVector.zero;
            }

            return this.FPVectorTable[key];
        }

        /**
         * @brief Gets a FPVector2 value.
         **/
        public FPVector2 GetFPVector2(byte key) {
            if (!this.FPVectorTable2.ContainsKey(key)) {
                return FPVector2.zero;
            }

            return this.FPVectorTable2[key];
        }

        /**
         * @brief Returns true if the key exists.
         **/
        public bool HasString(byte key) {
            return this.stringTable.ContainsKey(key);
        }

        /**
         * @brief Returns true if the key exists.
         **/
        public bool HasByte(byte key) {
            return this.byteTable.ContainsKey(key);
        }

        /**
         * @brief Returns true if the key exists.
         **/
        public bool HasByteArray(byte key) {
            return this.byteArrayTable.ContainsKey(key);
        }

        /**
         * @brief Returns true if the key exists.
         **/
        public bool HasInt(byte key) {
            return this.intTable.ContainsKey(key);
        }

        /**
         * @brief Returns true if the key exists.
         **/
        public bool HasFP(byte key) {
            return this.fpTable.ContainsKey(key);
        }

        /**
         * @brief Returns true if the key exists.
         **/
        public bool HasFPVector(byte key) {
            return this.FPVectorTable.ContainsKey(key);
        }

        /**
         * @brief Returns true if the key exists.
         **/
        public bool HasFPVector2(byte key) {
            return this.FPVectorTable2.ContainsKey(key);
        }

        public override string ToString()
        {
            string formatStr = string.Empty;

            formatStr += ("stringTable." + stringTable.Count + ":{");
            foreach (var str_item in stringTable)
            {
                formatStr += (str_item.Key + ":" + str_item.Value);
            }
            formatStr += "},";

            formatStr += ("byteTable." + byteTable.Count + ":{");
            foreach (var byte_item in byteTable)
            {
                formatStr += (byte_item.Key + ":" + byte_item.Value);
            }
            formatStr += "},";

            formatStr += ("intTable." + intTable.Count + ":{");
            foreach (var int_item in intTable)
            {
                formatStr += (int_item.Key + ":" + int_item.Value);
            }
            formatStr += "},";

            formatStr += ("fpTable." + fpTable.Count + ":{");
            foreach (var fp_item in fpTable)
            {
                formatStr += (fp_item.Key + ":" + fp_item.Value);
            }
            formatStr += "},";

            formatStr += ("byteArrayTable." + byteArrayTable.Count + ":{");
            foreach (var byteArray_item in byteArrayTable)
            {
                formatStr += (byteArray_item.Key + ":" + byteArray_item.Value);
            }
            formatStr += "},";

            formatStr += ("FPVectorTable." + FPVectorTable.Count + ":{");
            foreach (var FPVector_item in FPVectorTable)
            {
                formatStr += (FPVector_item.Key + ":" + FPVector_item.Value);
            }
            formatStr += "},";

            formatStr += ("FPVectorTable2." + FPVectorTable2.Count + ":{");
            foreach (var FPVector2_item in FPVectorTable2)
            {
                formatStr += (FPVector2_item.Key + ":" + FPVector2_item.Value);
            }
            formatStr += "},";

            return formatStr;
        }
    }

}