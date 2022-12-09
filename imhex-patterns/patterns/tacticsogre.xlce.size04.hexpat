#pragma MIME application/x-tacticsogre-xlce-size04
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
	// u32 data[parent.entrySize / 4] [[inline]];
};

bitfield XlceEntry_GlobalFlag_Storage {
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
	XlceEntry_GlobalFlag_Storage position [[inline]];
	u16 groupMask;
};

// it's impractical to infer the exact type just from data
// edit next line manually if you think it is above type
using XlceEntry = XlceEntry_Common;
Xlce xlce @ 0x00 [[inline]];
