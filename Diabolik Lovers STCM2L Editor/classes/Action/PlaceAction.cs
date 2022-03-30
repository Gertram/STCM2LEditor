using STCM2LEditor.classes.Action.Parameters;
using STCM2LEditor.utils;
using System.Collections.Generic;
using System.Linq;

namespace STCM2LEditor.classes.Action
{
    internal partial class PlaceAction : BaseStringAction
    {
        private readonly StringParameter ID;
        private readonly LocalParameter Par2;
        private readonly LocalParameter Par3;
        private PlaceAction() : base() { }
        private PlaceAction(StringParameter iD, LocalParameter par2, LocalParameter par3, StringParameter original, StringData translated, int address)
            : base(original, translated, address)
        {
            ID = iD;
            Par2 = par2;
            Par3 = par3;
        }
        public static PlaceAction ReadFromFile(byte[] file, ref int seek, ActionHeader header = null)
        {
            if (header == null)
            {
                header = ActionHeader.ReadFromFile(file, ref seek);
            }
            var id = StringParameter.ReadFromFile(file, ref seek);
            var par2 = LocalParameter.ReadFromFile(file, ref seek);
            var par3 = LocalParameter.ReadFromFile(file, ref seek);
            var original = StringParameter.ReadFromFile(file, ref seek);
            var translate = new StringData();
            var address = header.Address;
            var action = new PlaceAction(id, par2, par3, original, translate, address);

            action.InsertOriginal(file);

            return action;
        }
        public PlaceAction(StringData original, StringData translated) : base(original, translated)
        {
        }

        public override uint OpCode => ActionHelpers.ACTION_PLACE;
        protected override int ParametersOffset => base.ParametersOffset + IParameterHelpers.HEADER_LENGTH * 3;
        protected int MainLength => ActionHelpers.HEADER_LENGTH + ParametersOffset;
        protected override int OriginalDataOffset => MainLength + ID.Data.Length + Par2.Data.Length + Par3.Data.Length;
        public override int Address
        {
            get => base.Address;
            set
            {
                ID.Data.Address = value + MainLength;
                Par2.Data.Address = ID.Data.Address + ID.Data.Length;
                Par3.Data.Address = Par2.Data.Address + Par2.Data.Length;
                base.Address = value;
            }
        }
        private IReadOnlyList<IParameter> SelfParameters => new List<IParameter> { ID, Par2, Par3 };
        public override IReadOnlyList<IParameter> Parameters => SelfParameters.Concat(base.Parameters).ToList();

        public override byte[] ExtraData => base.ExtraData.Concat(ID.Data.ExtraData.Concat(Par2.Data.ExtraData).Concat(Par3.Data.ExtraData)).ToArray();

        protected override void WriteParameters(byte[] main, int position)
        {
            ID.Write(main, ref position);
            Par2.Write(main, ref position);
            Par3.Write(main, ref position);
            ByteUtil.InsertBytes(main, ID.Data.Write(), ID.Data.Address - Address);
            ByteUtil.InsertBytes(main, Par2.Data.Write(), Par2.Data.Address - Address);
            ByteUtil.InsertBytes(main, Par3.Data.Write(), Par3.Data.Address - Address);
            base.WriteParameters(main, position);
        }

        public override IStringAction Copy()
        {
            throw new System.NotImplementedException();
        }
    }
}
