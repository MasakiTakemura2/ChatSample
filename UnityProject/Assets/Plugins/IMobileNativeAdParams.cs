using System;

public class IMobileNativeAdParams
{
	public IMobileNativeAdParams ()
	{
		requestAdCount = 1;
		nativeImageGetFlag = true;
	}

	public int requestAdCount{ get; set; }
	public bool nativeImageGetFlag{ get; set; }
}
