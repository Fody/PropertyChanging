.method public hidebysig specialname 
	instance void set_IsFlag (
		bool 'value'
	) cil managed 
{
	// Header size: 12
	// Code size: 38 (0x26)
	.maxstack 2

	IL_0000: ldarg.0
	IL_0001: call instance bool ClassWithNullableBackingField::get_IsFlag()
	IL_0006: ldarg.1
	IL_0007: ceq
	IL_0009: brfalse.s IL_000c

	IL_000b: ret

	IL_000c: nop
	IL_000d: ldarg.0
	IL_000e: ldstr "IsFlag"
	IL_0013: callvirt instance void ClassWithNullableBackingField::OnPropertyChanging(string)
	IL_0018: ldarg.0
	IL_0019: ldarg.1
	IL_001a: newobj instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
	IL_001f: stfld valuetype [mscorlib]System.Nullable`1<bool> ClassWithNullableBackingField::_isFlag
	IL_0024: nop
	IL_0025: ret
} // end of method ClassWithNullableBackingField::set_IsFlag
