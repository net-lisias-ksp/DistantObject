/*
		This file is part of Distant Object Enhancement /L
			© 2021-2023 LisiasT
			© 2019-2021 TheDarkBadger
			© 2014-2019 MOARdV
			© 2014 Rubber Ducky

		Distant Object Enhancement /L is double licensed, as follows:

		* SKL 1.0 : https://ksp.lisias.net/SKL-1_0.txt
		* GPL 2.0 : https://www.gnu.org/licenses/gpl-2.0.txt

		And you are allowed to choose the License that better suit your needs.

		Distant Object Enhancement /L is distributed in the hope that it will
		be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
		of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

		You should have received a copy of the SKL Standard License 1.0
		along with Distant Object Enhancement /L.
		If not, see <https://ksp.lisias.net/SKL-1_0.txt>.

		You should have received a copy of the GNU General Public License 2.0
		along with Distant Object Enhancement /L.
		If not, see <https://www.gnu.org/licenses/>.
*/
using UnityEngine;

namespace DistantObject
{
    class Utility
    {
        public static Vector4 RGB2HSL(Color rgba)
        {
            float h = 0.0f, s = 0.0f, l = 0.0f;
            float r = rgba.r;
            float g = rgba.g;
            float b = rgba.b;

            float v;
            float m;
            float vm;

            float r2, g2, b2;


            v = Mathf.Max(r, g);
            v = Mathf.Max(v, b);

            m = Mathf.Min(r, g);
            m = Mathf.Min(m, b);

            l = (m + v) / 2.0f;

            if (l <= 0.0f)
            {
                return new Vector4(0.0f, 0.0f, 0.0f, rgba.a);
            }

            vm = v - m;

            s = vm;

            if (s > 0.0f)
            {
                s /= (l <= 0.5f) ? (v + m) : (2.0f - v - m);
            }
            else
            {
                return new Vector4(0.0f, 0.0f, l, rgba.a);
            }

            r2 = (v - r) / vm;
            g2 = (v - g) / vm;
            b2 = (v - b) / vm;

            if (r == v)
            {
                h = (g == m ? 5.0f + b2 : 1.0f - g2);
            }
            else if (g == v)
            {
                h = (b == m ? 1.0f + r2 : 3.0f - b2);
            }
            else
            {
                h = (r == m ? 3.0f + g2 : 5.0f - r2);
            }

            h /= 6.0f;

            return new Vector4(h, s, l, rgba.a);
        }

    }
}
