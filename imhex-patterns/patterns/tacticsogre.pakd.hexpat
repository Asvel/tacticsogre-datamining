#pragma MIME application/x-tacticsogre-pakd
#pragma endian little

u32 pakdBegin = 0;

struct Offset {
	u32 offset;
	char signature[4] @ pakdBegin + offset;
};

struct Pakd {
	pakdBegin = $;
	char signature[4];
	u32 count;
	Offset offsets[count];
	u32 dataEnd;
	if (pakdBegin + offsets[0].offset >= $ + count * 4) {
		u32 ids[count];
	}
};

Pakd pakd @ 0x00 [[inline]];
