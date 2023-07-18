﻿using System;
using System.Collections.Generic;

namespace ModernThemables.Icons
{
    public static class IconDataFactory
    {
        public static Lazy<IDictionary<IconType, (string, bool)>> DataIndex { get; }

        static IconDataFactory()
        {
            if (DataIndex == null)
            {
                DataIndex = new Lazy<IDictionary<IconType, (string, bool)>>(Create);
            }
        }

        public static IDictionary<IconType, (string, bool)> Create()
        {
            return new Dictionary<IconType, (string, bool)>
            {
                { IconType.None, ("", false) },
                { IconType.Bin, ("M256 661.333h-42.667v-554.667c0-47.128 38.205-85.333 85.333-85.333v0h426.667c47.128 0 85.333 38.205 85.333 85.333v0 554.667h-554.667zM709.035 789.333l-69.035 85.333h-256l-69.035-85.333h-186.965v-85.333h768v85.333z", true) },
                { IconType.Building, ("M11.5,1L2,6V8H21V6M16,10V17H19V10M2,22H21V19H2M10,10V17H13V10M4,10V17H7V10H4z", false) },
                { IconType.Calculator, ("M256 21.333h512c47.128 0 85.333 38.205 85.333 85.333v0 682.667c0 47.128-38.205 85.333-85.333 85.333v0h-512c-47.128 0-85.333-38.205-85.333-85.333v0-682.667c0-47.128 38.205-85.333 85.333-85.333v0zM384 149.333h-85.333v85.333h85.333v-85.333zM384 320h-85.333v85.333h85.333v-85.333zM384 490.667h-85.333v85.333h85.333v-85.333zM554.667 149.333h-85.333v85.333h85.333v-85.333zM554.667 320h-85.333v85.333h85.333v-85.333zM554.667 490.667h-85.333v85.333h85.333v-85.333zM725.333 149.333h-85.333v256h85.333v-256zM725.333 490.667h-85.333v85.333h85.333v-85.333zM256 789.333h512v-128h-512v128z", true) },
                { IconType.Calendar, ("M 18.0025,57.0081L 18.0025,23.0032L 23.0032,23.0032L 23.0032,20.0028C 23.0033,18.898 23.8988,18.0025 25.0035,18.0025L 29.004,18.0025C 30.1087,18.0025 31.0042,18.898 31.0043,20.0026L 31.0043,23.0032L 45.0063,23.0032L 45.0062,20.0026C 45.0062,18.8978 45.9018,18.0023 47.0065,18.0023L 51.0071,18.0023C 52.1118,18.0023 53.0074,18.8978 53.0074,20.0026L 53.0074,23.0032L 58.0081,23.0032L 58.0081,57.0081L 18.0025,57.0081 Z M 21.0029,54.0077L 55.0076,54.0077L 55.0076,31.0044L 21.0029,31.0044L 21.0029,54.0077 Z M 23.0032,47.0066L 29.004,47.0066L 29.004,52.0073L 23.0032,52.0073L 23.0032,47.0066 Z M 31.0043,47.0066L 37.0051,47.0066L 37.0051,52.0073L 31.0043,52.0073L 31.0043,47.0066 Z M 39.0054,47.0066L 45.0063,47.0066L 45.0063,52.0073L 39.0054,52.0073L 39.0054,47.0066 Z M 47.0065,47.0066L 53.0074,47.0066L 53.0074,52.0073L 47.0065,52.0073L 47.0065,47.0066 Z M 23.0032,40.0056L 29.004,40.0056L 29.004,45.0063L 23.0032,45.0063L 23.0032,40.0056 Z M 31.0043,40.0056L 37.0051,40.0056L 37.0051,45.0063L 31.0043,45.0063L 31.0043,40.0056 Z M 39.0054,40.0056L 45.0063,40.0056L 45.0063,45.0063L 39.0054,45.0063L 39.0054,40.0056 Z M 47.0065,40.0056L 53.0074,40.0056L 53.0074,45.0063L 47.0065,45.0063L 47.0065,40.0056 Z M 31.0043,33.0048L 37.0051,33.0048L 37.0051,38.0055L 31.0043,38.0055L 31.0043,33.0048 Z M 39.0054,33.0048L 45.0063,33.0048L 45.0063,38.0055L 39.0054,38.0055L 39.0054,33.0048 Z M 47.0065,33.0048L 53.0074,33.0048L 53.0074,38.0055L 47.0065,38.0055L 47.0065,33.0048 Z M 48.5067,20.0028C 47.6782,20.0028 47.0065,20.6745 47.0065,21.5031L 47.0065,24.5035C 47.0065,25.332 47.6782,26.0037 48.5067,26.0037L 49.5069,26.0037C 50.3354,26.0037 51.0071,25.332 51.0071,24.5035L 51.0071,21.5031C 51.0071,20.6745 50.3354,20.0028 49.5069,20.0028L 48.5067,20.0028 Z M 26.5037,20.0028C 25.6751,20.0028 25.0035,20.6745 25.0035,21.503L 25.0035,24.5034C 25.0035,25.332 25.6751,26.0037 26.5037,26.0037L 27.5038,26.0037C 28.3324,26.0037 29.004,25.332 29.004,24.5034L 29.004,21.503C 29.004,20.6745 28.3324,20.0028 27.5038,20.0028L 26.5037,20.0028 Z ", false) },
                { IconType.Close, ("M88.95 100L31.975 43.025L43.025 31.975L100 88.95L156.975 31.975L168.025 43.025L111.05 100L168.025 156.975L156.975 168.025L100 111.05L43.025 168.025L31.975 156.975L88.95 100z", false) },
                { IconType.Collapse, ("M30.8625 212.8875A9.375 9.375 0 0 0 44.1375 212.8875L150 107.00625L255.8625 212.8875A9.375 9.375 0 0 0 269.1375000000001 199.6125L156.6375 87.1125A9.375 9.375 0 0 0 143.3625 87.1125L30.8625 199.6125A9.375 9.375 0 0 0 30.8625 212.8875z", false) },
                { IconType.Download, ("M5,20H19V18H5M19,9H15V3H9V9H5L12,16L19,9z", false) },
                { IconType.Expand, ("M143.3625 212.8875A9.375 9.375 0 0 0 156.6375 212.8875L269.1375 100.3875A9.375 9.375 0 0 0 255.8625 87.1125L150 192.99375L44.1375 87.1125A9.375 9.375 0 0 0 30.8625 100.3875L143.3625 212.8875z", false) },
                { IconType.Gear, ("M12,15.5A3.5,3.5 0 0,1 8.5,12A3.5,3.5 0 0,1 12,8.5A3.5,3.5 0 0,1 15.5,12A3.5,3.5 0 0,1 12,15.5M19.43,12.97C19.47,12.65 19.5,12.33 19.5,12C19.5,11.67 19.47,11.34 19.43,11L21.54,9.37C21.73,9.22 21.78,8.95 21.66,8.73L19.66,5.27C19.54,5.05 19.27,4.96 19.05,5.05L16.56,6.05C16.04,5.66 15.5,5.32 14.87,5.07L14.5,2.42C14.46,2.18 14.25,2 14,2H10C9.75,2 9.54,2.18 9.5,2.42L9.13,5.07C8.5,5.32 7.96,5.66 7.44,6.05L4.95,5.05C4.73,4.96 4.46,5.05 4.34,5.27L2.34,8.73C2.21,8.95 2.27,9.22 2.46,9.37L4.57,11C4.53,11.34 4.5,11.67 4.5,12C4.5,12.33 4.53,12.65 4.57,12.97L2.46,14.63C2.27,14.78 2.21,15.05 2.34,15.27L4.34,18.73C4.46,18.95 4.73,19.03 4.95,18.95L7.44,17.94C7.96,18.34 8.5,18.68 9.13,18.93L9.5,21.58C9.54,21.82 9.75,22 10,22H14C14.25,22 14.46,21.82 14.5,21.58L14.87,18.93C15.5,18.67 16.04,18.34 16.56,17.94L19.05,18.95C19.27,19.03 19.54,18.95 19.66,18.73L21.66,15.27C21.78,15.05 21.73,14.78 21.54,14.63L19.43,12.97z", false) },
                { IconType.Graph, ("M320 256h192v-320h-192v320zM64 64h192v-128h-192v128zM832 192h192v-256h-192v256zM576 384h192v-448h-192v448zM1024 955.52l-363.52-318.080-350.080 108.8-310.4-240.64v-121.6l329.6 256 353.92-110.080 340.48 298.24v127.36z", false) },
                { IconType.List, ("M336 64h-80c0-35.3-28.7-64-64-64s-64 28.7-64 64H48C21.5 64 0 85.5 0 112v352c0 26.5 21.5 48 48 48h288c26.5 0 48-21.5 48-48V112c0-26.5-21.5-48-48-48zM96 424c-13.3 0-24-10.7-24-24s10.7-24 24-24 24 10.7 24 24-10.7 24-24 24zm0-96c-13.3 0-24-10.7-24-24s10.7-24 24-24 24 10.7 24 24-10.7 24-24 24zm0-96c-13.3 0-24-10.7-24-24s10.7-24 24-24 24 10.7 24 24-10.7 24-24 24zm96-192c13.3 0 24 10.7 24 24s-10.7 24-24 24-24-10.7-24-24 10.7-24 24-24zm128 368c0 4.4-3.6 8-8 8H168c-4.4 0-8-3.6-8-8v-16c0-4.4 3.6-8 8-8h144c4.4 0 8 3.6 8 8v16zm0-96c0 4.4-3.6 8-8 8H168c-4.4 0-8-3.6-8-8v-16c0-4.4 3.6-8 8-8h144c4.4 0 8 3.6 8 8v16zm0-96c0 4.4-3.6 8-8 8H168c-4.4 0-8-3.6-8-8v-16c0-4.4 3.6-8 8-8h144c4.4 0 8 3.6 8 8v16z", false) },
                { IconType.MagnifyingGlass, ("M443.5 420.2L336.7 312.4c20.9-26.2 33.5-59.4 33.5-95.5 0-84.5-68.5-153-153.1-153S64 132.5 64 217s68.5 153 153.1 153c36.6 0 70.1-12.8 96.5-34.2l106.1 107.1c3.2 3.4 7.6 5.1 11.9 5.1 4.1 0 8.2-1.5 11.3-4.5 6.6-6.3 6.8-16.7.6-23.3zm-226.4-83.1c-32.1 0-62.3-12.5-85-35.2-22.7-22.7-35.2-52.9-35.2-84.9 0-32.1 12.5-62.3 35.2-84.9 22.7-22.7 52.9-35.2 85-35.2s62.3 12.5 85 35.2c22.7 22.7 35.2 52.9 35.2 84.9 0 32.1-12.5 62.3-35.2 84.9-22.7 22.7-52.9 35.2-85 35.2z", false) },
                { IconType.Maximise, ("M200 1050H1000A50 50 0 0 0 1050 1000V200A50 50 0 0 0 1000 150H200A50 50 0 0 0 150 200V1000A50 50 0 0 0 200 1050zM250 950V250H950V950H250z", false) },
                { IconType.Menu, ("M3,8H21a1,1,0,0,0,0-2H3A1,1,0,0,0,3,8Zm18,8H3a1,1,0,0,0,0,2H21a1,1,0,0,0,0-2Zm0-5H3a1,1,0,0,0,0,2H21a1,1,0,0,0,0-2z", false) },
                { IconType.Minimise, ("M175 100V87.5H37.5V100H175z", false) },
                { IconType.Monitor, ("M21,14V4H3V14H21M21,2A2,2 0 0,1 23,4V16A2,2 0 0,1 21,18H14L16,21V22H8V21L10,18H3C1.89,18 1,17.1 1,16V4C1,2.89 1.89,2 3,2H21M4,5H15V10H4V5M16,5H20V7H16V5M20,8V13H16V8H20M4,11H9V13H4V11M10,11H15V13H10V11z", false) },
                { IconType.Moon, ("M307.5 50H305A257.75000000000006 257.75000000000006 0 0 0 121.5 128.75A261.5 261.5 0 0 0 115 478.75A253.25 253.25 0 0 0 215 547.25A25 25 0 0 0 241.5000000000001 541.75A25 25 0 0 0 247.5000000000001 516.75A210 210 0 0 1 296.0000000000001 296.5A211.75 211.75 0 0 1 516.75 248A25 25 0 0 0 548.5 215.75A254 254 0 0 0 490.0000000000001 125A257 257 0 0 0 307.5 50z", true) },
                { IconType.OpenFile, ("M1879 584c0 -24 -15 -48 -31 -66l-336 -396c-58 -68 -176 -122 -264 -122h-1088c-36 0 -87 11 -87 56c0 24 15 48 31 66l336 396c58 68 176 122 264 122h1088c36 0 87 -11 87 -56zM1536 928v-160h-832c-125 0 -280 -71 -361 -167l-337 -396l-5 -6c0 8 -1 17 -1 25v960 c0 123 101 224 224 224h320c123 0 224 -101 224 -224v-32h544c123 0 224 -101 224 -224z", false) },
                { IconType.Pen, ("M12.3 3.7l4 4L4 20H0v-4L12.3 3.7zm1.4-1.4L16 0l4 4-2.3 2.3-4-4z", false) },
                { IconType.Pin, ("M184.275 286.4625A9.375 9.375 0 0 0 190.9125 283.725L283.725 190.9125A9.375 9.375 0 0 0 283.725 177.65625C274.725 168.65625 263.625 166.63125 255.54375 166.63125C252.225 166.63125 249.2625 166.96875 246.91875 167.3625L188.15625 108.6A111.13125 111.13125 0 0 0 191.15625 89.60625C192.0187499999999 76.44375 190.55625 57.975 177.6562499999999 45.075A9.375 9.375 0 0 0 164.3999999999999 45.075L111.3562499999999 98.1L51.6937499999999 38.4375C48.0374999999999 34.78125 28.8374999999999 21.525 25.1812499999999 25.18125C21.5249999999999 28.8375 34.7812499999999 48.05625 38.4374999999999 51.69375L98.0999999999999 111.35625L45.0749999999999 164.4A9.375 9.375 0 0 0 45.0749999999999 177.65625C57.9749999999999 190.55625 76.4437499999999 192.0375 89.6062499999999 191.15625A111.0375 111.0375 0 0 0 108.5999999999999 188.15625L167.3624999999999 246.9A51.975 51.975 0 0 0 166.6124999999999 255.54375C166.6124999999999 263.60625 168.6374999999999 274.70625 177.6562499999999 283.725A9.375 9.375 0 0 0 184.2749999999999 286.4625z", true) },
                { IconType.Play, ("M0.038 888.94v-924.545c2.229-28.184 25.649-50.211 54.214-50.211 9.594 0 18.608 2.485 26.432 6.846l-0.276-0.141 914.869 460.794c17.066 8.179 28.642 25.316 28.642 45.158s-11.576 36.979-28.343 45.028l-0.3 0.13-914.869 460.794c-7.55 4.226-16.567 6.715-26.166 6.715-28.683 0-52.175-22.225-54.194-50.393l-0.010-0.175z", false) },
                { IconType.Plus, ("M232 280L64 280 64 232 232 232 232 64 280 64 280 232 448 232 448 280 280 280 280 448 232 448 232 280z", false) },
                { IconType.Restore, ("M37.5 137.5V25H150V137.5H37.5zM137.5 37.5H50V125H137.5V37.5zM62.5 137.5H75V150H162.5V62.5H150V50H175V162.5H62.5V137.5z", true) },
                { IconType.Save, ("M 433.9,130.1 382,78.2 C 373,69.2 360.7,64 347.9,64 h -28 c -8.8,0 -16,7.3 -16,16.2 v 80 c 0,8.8 -7.2,16 -16,16 H 160 c -8.8,0 -16,-7.2 -16,-16 v -80 C 144,71.4 136.8,64 128,64 H 96 C 78.4,64 64,78.4 64,96 v 320 c 0,17.6 14.4,32 32,32 h 320 c 17.6,0 32,-14.4 32,-32 V 164 c 0,-12.7 -5.1,-24.9 -14.1,-33.9 z M 322,400.1 c 0,8.8 -8,16 -17.8,16 H 143.8 c -9.8,0 -17.8,-7.2 -17.8,-16 v -96 c 0,-8.8 8,-16 17.8,-16 h 160.4 c 9.8,0 17.8,7.2 17.8,16 z", false) },
                { IconType.Sun, ("M300 450A25 25 0 0 1 325 475V525A25 25 0 0 1 275 525V475A25 25 0 0 1 300 450zM525 325H475A25 25 0 0 1 475 275H525A25 25 0 0 1 525 325zM150 300A25 25 0 0 1 125 325H75A25 25 0 0 1 75 275H125A25 25 0 0 1 150 300zM155.5 475A25 25 0 0 1 120.75 438.25L156.75 403.5A25 25 0 0 1 175 396.5A25 25 0 0 1 193 404.25A25 25 0 0 1 193 439.5zM425 396.5A25 25 0 0 1 442.2500000000001 403.5L478.2500000000001 438.25A25 25 0 0 1 444.5 475L408.5 439.5A25 25 0 0 1 408.5 404.25A25 25 0 0 1 425 396.5zM300 150A25 25 0 0 1 275 125V75A25 25 0 0 1 325 75V125A25 25 0 0 1 300 150zM443.25 196.5A25 25 0 0 1 408.5 160.5L444.5 125A25 25 0 0 1 461.7500000000001 118A25 25 0 0 1 479.7500000000001 125.5A25 25 0 0 1 479.7500000000001 161zM156.75 196.5L120.75 161.75A25 25 0 0 1 120.75 126.25A25 25 0 0 1 138.75 118.7499999999999A25 25 0 0 1 155.5 124.9999999999999L191.5 159.75A25 25 0 0 1 156.75 195.75zM300 400A100 100 0 1 1 400 300A100 100 0 0 1 300 400z", false) },
                { IconType.Tag, ("M416 64H257.6L76.5 251.6c-8 8-12.3 18.5-12.5 29-.3 11.3 3.9 22.6 12.5 31.2l123.7 123.6c8 8 20.8 12.5 28.8 12.5s22.8-3.9 31.4-12.5L448 256V96l-32-32zm-30.7 102.7c-21.7 6.1-41.3-10-41.3-30.7 0-17.7 14.3-32 32-32 20.7 0 36.8 19.6 30.7 41.3-2.9 10.3-11.1 18.5-21.4 21.4z", false) },
                { IconType.Tick, ("M64 261L96 229 149 282 250 160 286 192 153 350 64 261z", false) },
            };
        }
    }
}