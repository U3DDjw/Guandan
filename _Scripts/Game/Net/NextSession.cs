namespace Net
{
	public class NextSession
	{
		private int _Seed;
		
		public NextSession()
		{
			this._Seed = 1;
		}

		public NextSession( int _rhs )
		{
			this._Seed = _rhs;
		}
		
		private enum NextEnum { SessionMax = 0x7fff };
		
		public int next()
		{
			return ((_Seed = _Seed * (int)214013L + (int)2531011L) >> 16) & 0x7fff;
		}

		public void set_seed(int seed)
		{
			_Seed = seed;
		}

		// generate a random value in [0, mod]
		public int rand(int mod)
		{
			return next()*(mod+1)/((int)NextEnum.SessionMax+1);
		}
	}

}


