#pragma MIME application/x-tacticsogre-xlce
#pragma endian little

using XlceEntry;

struct Xlce {
	char signature[4];
	u32 count;
	u32 bodyStart;
	u32 entrySize;
	XlceEntry entries[count] @ bodyStart [[inline]];
};

struct XlceEntry_Common {
	u8 data[parent.entrySize] [[inline]];
};

struct XlceEntry_Common32 {
	u32 data[parent.entrySize / 4] [[inline]];
};

struct XlceEntry_Common64 {
	u32 data[parent.entrySize / 8] [[inline]];
};

bitfield XlceEntry__GlobalFlagStorage {
    sizeType : 4;
    if (sizeType == 1) {
        bitPosition: 3;
        bytePosition: 9;
    }
    else if (sizeType == 2) {
        bitPosition: 1;
        bytePosition: 11;
    }
    else if (sizeType == 3) {
        bytePosition: 12;
    }
    else if (sizeType == 4) {
        bytePosition: 12;
    }
    else {
        bytePosition: 12;
    }
};
struct XlceEntry_GlobalFlag {
	XlceEntry__GlobalFlagStorage position [[inline]];
	u16 groupMask;
};

using XlceEntry = XlceEntry_Common;
Xlce xlce @ 0x00 [[inline]];
