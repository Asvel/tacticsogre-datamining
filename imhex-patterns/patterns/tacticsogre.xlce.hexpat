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
	// u32 data[parent.entrySize / 4] [[inline]];
};

using XlceEntry = XlceEntry_Common;
Xlce xlce @ 0x00 [[inline]];
