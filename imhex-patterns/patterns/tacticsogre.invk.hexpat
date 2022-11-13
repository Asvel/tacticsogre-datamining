#pragma MIME application/x-tacticsogre-invk
#pragma endian little

#include <std/mem.pat>
#include <tacticsogre/screenplay-instruction.pat>

struct InvkEntry {
	u16 invkId;
	Instruction instructions[while(std::mem::read_signed($, 2) != -1)] [[inline]];
    padding[2];
};

struct InvkGroup {
	u16 count;
	InvkEntry entries[count] [[inline]];
};

struct InvkIndex {
	u32 offset;
	InvkGroup value @ offset [[inline]];
};

struct Invk {
	char signature[4];
	u16 count;
	padding[2];
	InvkIndex groups[count] [[inline]];
};

Invk invk @ 0x00 [[inline]];
