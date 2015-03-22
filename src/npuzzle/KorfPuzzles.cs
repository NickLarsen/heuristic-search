﻿using System;
using System.Linq;

namespace npuzzle
{
    class KorfPuzzles
    {
        public static KorfPuzzle[] Puzzles =
        {
            new KorfPuzzle { Number = 1, InitialState = new byte[] { 14, 13, 15, 7, 11, 12, 9, 5, 6, 0, 2, 1, 4, 8, 10, 3 }, Actual = 57, KorfNodesExpanded = 276361933 },
            new KorfPuzzle { Number = 2, InitialState = new byte[] { 13, 5, 4, 10, 9, 12, 8, 14, 2, 3, 7, 1, 0, 15, 11, 6 }, Actual = 55, KorfNodesExpanded = 15300442 },
            new KorfPuzzle { Number = 3, InitialState = new byte[] { 14, 7, 8, 2, 13, 11, 10, 4, 9, 12, 5, 0, 3, 6, 1, 15 }, Actual = 59, KorfNodesExpanded = 565994203 },
            new KorfPuzzle { Number = 4, InitialState = new byte[] { 5, 12, 10, 7, 15, 11, 14, 0, 8, 2, 1, 13, 3, 4, 9, 6 }, Actual = 56, KorfNodesExpanded = 62643179 },
            new KorfPuzzle { Number = 5, InitialState = new byte[] { 4, 7, 14, 13, 10, 3, 9, 12, 11, 5, 6, 15, 1, 2, 8, 0 }, Actual = 56, KorfNodesExpanded = 11020325 },
            new KorfPuzzle { Number = 6, InitialState = new byte[] { 14, 7, 1, 9, 12, 3, 6, 15, 8, 11, 2, 5, 10, 0, 4, 13 }, Actual = 52, KorfNodesExpanded = 32201660 },
            new KorfPuzzle { Number = 7, InitialState = new byte[] { 2, 11, 15, 5, 13, 4, 6, 7, 12, 8, 10, 1, 9, 3, 14, 0 }, Actual = 50, KorfNodesExpanded = 387138094 },
            new KorfPuzzle { Number = 8, InitialState = new byte[] { 12, 11, 15, 3, 8, 0, 4, 2, 6, 13, 9, 5, 14, 1, 10, 7 }, Actual = 50, KorfNodesExpanded = 39118937 },
            new KorfPuzzle { Number = 9, InitialState = new byte[] { 3, 14, 9, 11, 5, 4, 8, 2, 13, 12, 6, 7, 10, 1, 15, 0 }, Actual = 46, KorfNodesExpanded = 1650696 },
            new KorfPuzzle { Number = 10, InitialState = new byte[] { 13, 11, 8, 9, 0, 15, 7, 10, 4, 3, 6, 14, 5, 12, 2, 1 }, Actual = 59, KorfNodesExpanded = 198758703 },
            new KorfPuzzle { Number = 11, InitialState = new byte[] { 5, 9, 13, 14, 6, 3, 7, 12, 10, 8, 4, 0, 15, 2, 11, 1 }, Actual = 57, KorfNodesExpanded = 150346072 },
            new KorfPuzzle { Number = 12, InitialState = new byte[] { 14, 1, 9, 6, 4, 8, 12, 5, 7, 2, 3, 0, 10, 11, 13, 15 }, Actual = 45, KorfNodesExpanded = 546344 },
            new KorfPuzzle { Number = 13, InitialState = new byte[] { 3, 6, 5, 2, 10, 0, 15, 14, 1, 4, 13, 12, 9, 8, 11, 7 }, Actual = 46, KorfNodesExpanded = 11861705 },
            new KorfPuzzle { Number = 14, InitialState = new byte[] { 7, 6, 8, 1, 11, 5, 14, 10, 3, 4, 9, 13, 15, 2, 0, 12 }, Actual = 59, KorfNodesExpanded = 1369596778 },
            new KorfPuzzle { Number = 15, InitialState = new byte[] { 13, 11, 4, 12, 1, 8, 9, 15, 6, 5, 14, 2, 7, 3, 10, 0 }, Actual = 62, KorfNodesExpanded = 543598067 },
            new KorfPuzzle { Number = 16, InitialState = new byte[] { 1, 3, 2, 5, 10, 9, 15, 6, 8, 14, 13, 11, 12, 4, 7, 0 }, Actual = 42, KorfNodesExpanded = 17984051 },
            new KorfPuzzle { Number = 17, InitialState = new byte[] { 15, 14, 0, 4, 11, 1, 6, 13, 7, 5, 8, 9, 3, 2, 10, 12 }, Actual = 66, KorfNodesExpanded = 607399560 },
            new KorfPuzzle { Number = 18, InitialState = new byte[] { 6, 0, 14, 12, 1, 15, 9, 10, 11, 4, 7, 2, 8, 3, 5, 13 }, Actual = 55, KorfNodesExpanded = 23711067 },
            new KorfPuzzle { Number = 19, InitialState = new byte[] { 7, 11, 8, 3, 14, 0, 6, 15, 1, 4, 13, 9, 5, 12, 2, 10 }, Actual = 46, KorfNodesExpanded = 1280495 },
            new KorfPuzzle { Number = 20, InitialState = new byte[] { 6, 12, 11, 3, 13, 7, 9, 15, 2, 14, 8, 10, 4, 1, 5, 0 }, Actual = 52, KorfNodesExpanded = 17954870 },
            new KorfPuzzle { Number = 21, InitialState = new byte[] { 12, 8, 14, 6, 11, 4, 7, 0, 5, 1, 10, 15, 3, 13, 9, 2 }, Actual = 54, KorfNodesExpanded = 257064810 },
            new KorfPuzzle { Number = 22, InitialState = new byte[] { 14, 3, 9, 1, 15, 8, 4, 5, 11, 7, 10, 13, 0, 2, 12, 6 }, Actual = 59, KorfNodesExpanded = 750746755 }, // misprint
            new KorfPuzzle { Number = 23, InitialState = new byte[] { 10, 9, 3, 11, 0, 13, 2, 14, 5, 6, 4, 7, 8, 15, 1, 12 }, Actual = 49, KorfNodesExpanded = 15971319 },
            new KorfPuzzle { Number = 24, InitialState = new byte[] { 7, 3, 14, 13, 4, 1, 10, 8, 5, 12, 9, 11, 2, 15, 6, 0 }, Actual = 54, KorfNodesExpanded = 42693209 },
            new KorfPuzzle { Number = 25, InitialState = new byte[] { 11, 4, 2, 7, 1, 0, 10, 15, 6, 9, 14, 8, 3, 13, 5, 12 }, Actual = 52, KorfNodesExpanded = 100734844 },
            new KorfPuzzle { Number = 26, InitialState = new byte[] { 5, 7, 3, 12, 15, 13, 14, 8, 0, 10, 9, 6, 1, 4, 2, 11 }, Actual = 58, KorfNodesExpanded = 226668645 },
            new KorfPuzzle { Number = 27, InitialState = new byte[] { 14, 1, 8, 15, 2, 6, 0, 3, 9, 12, 10, 13, 4, 7, 5, 11 }, Actual = 53, KorfNodesExpanded = 306123421 },
            new KorfPuzzle { Number = 28, InitialState = new byte[] { 13, 14, 6, 12, 4, 5, 1, 0, 9, 3, 10, 2, 15, 11, 8, 7 }, Actual = 52, KorfNodesExpanded = 5934442 },
            new KorfPuzzle { Number = 29, InitialState = new byte[] { 9, 8, 0, 2, 15, 1, 4, 14, 3, 10, 7, 5, 11, 13, 6, 12 }, Actual = 54, KorfNodesExpanded = 117076111 },
            new KorfPuzzle { Number = 30, InitialState = new byte[] { 12, 15, 2, 6, 1, 14, 4, 8, 5, 3, 7, 0, 10, 13, 9, 11 }, Actual = 47, KorfNodesExpanded = 2196593 },
            new KorfPuzzle { Number = 31, InitialState = new byte[] { 12, 8, 15, 13, 1, 0, 5, 4, 6, 3, 2, 11, 9, 7, 14, 10 }, Actual = 50, KorfNodesExpanded = 2351811 },
            new KorfPuzzle { Number = 32, InitialState = new byte[] { 14, 10, 9, 4, 13, 6, 5, 8, 2, 12, 7, 0, 1, 3, 11, 15 }, Actual = 59, KorfNodesExpanded = 661041936 },
            new KorfPuzzle { Number = 33, InitialState = new byte[] { 14, 3, 5, 15, 11, 6, 13, 9, 0, 10, 2, 12, 4, 1, 7, 8 }, Actual = 60, KorfNodesExpanded = 480637867 },
            new KorfPuzzle { Number = 34, InitialState = new byte[] { 6, 11, 7, 8, 13, 2, 5, 4, 1, 10, 3, 9, 14, 0, 12, 15 }, Actual = 52, KorfNodesExpanded = 20671552 },
            new KorfPuzzle { Number = 35, InitialState = new byte[] { 1, 6, 12, 14, 3, 2, 15, 8, 4, 5, 13, 9, 0, 7, 11, 10 }, Actual = 55, KorfNodesExpanded = 47506056 },
            new KorfPuzzle { Number = 36, InitialState = new byte[] { 12, 6, 0, 4, 7, 3, 15, 1, 13, 9, 8, 11, 2, 14, 5, 10 }, Actual = 52, KorfNodesExpanded = 59802602 },
            new KorfPuzzle { Number = 37, InitialState = new byte[] { 8, 1, 7, 12, 11, 0, 10, 5, 9, 15, 6, 13, 14, 2, 3, 4 }, Actual = 58, KorfNodesExpanded = 280078791 },
            new KorfPuzzle { Number = 38, InitialState = new byte[] { 7, 15, 8, 2, 13, 6, 3, 12, 11, 0, 4, 10, 9, 5, 1, 14 }, Actual = 53, KorfNodesExpanded = 24492852 },
            new KorfPuzzle { Number = 39, InitialState = new byte[] { 9, 0, 4, 10, 1, 14, 15, 3, 12, 6, 5, 7, 11, 13, 8, 2 }, Actual = 49, KorfNodesExpanded = 19355806 },
            new KorfPuzzle { Number = 40, InitialState = new byte[] { 11, 5, 1, 14, 4, 12, 10, 0, 2, 7, 13, 3, 9, 15, 6, 8 }, Actual = 54, KorfNodesExpanded = 63276188 },
            new KorfPuzzle { Number = 41, InitialState = new byte[] { 8, 13, 10, 9, 11, 3, 15, 6, 0, 1, 2, 14, 12, 5, 4, 7 }, Actual = 54, KorfNodesExpanded = 51501544 },
            new KorfPuzzle { Number = 42, InitialState = new byte[] { 4, 5, 7, 2, 9, 14, 12, 13, 0, 3, 6, 11, 8, 1, 15, 10 }, Actual = 42, KorfNodesExpanded = 877823 },
            new KorfPuzzle { Number = 43, InitialState = new byte[] { 11, 15, 14, 13, 1, 9, 10, 4, 3, 6, 2, 12, 7, 5, 8, 0 }, Actual = 64, KorfNodesExpanded = 41124767 },
            new KorfPuzzle { Number = 44, InitialState = new byte[] { 12, 9, 0, 6, 8, 3, 5, 14, 2, 4, 11, 7, 10, 1, 15, 13 }, Actual = 50, KorfNodesExpanded = 95733125 },
            new KorfPuzzle { Number = 45, InitialState = new byte[] { 3, 14, 9, 7, 12, 15, 0, 4, 1, 8, 5, 6, 11, 10, 2, 13 }, Actual = 51, KorfNodesExpanded = 6158733 },
            new KorfPuzzle { Number = 46, InitialState = new byte[] { 8, 4, 6, 1, 14, 12, 2, 15, 13, 10, 9, 5, 3, 7, 0, 11 }, Actual = 49, KorfNodesExpanded = 22119320 },
            new KorfPuzzle { Number = 47, InitialState = new byte[] { 6, 10, 1, 14, 15, 8, 3, 5, 13, 0, 2, 7, 4, 9, 11, 12 }, Actual = 47, KorfNodesExpanded = 1411294 },
            new KorfPuzzle { Number = 48, InitialState = new byte[] { 8, 11, 4, 6, 7, 3, 10, 9, 2, 12, 15, 13, 0, 1, 5, 14 }, Actual = 49, KorfNodesExpanded = 1905023 },
            new KorfPuzzle { Number = 49, InitialState = new byte[] { 10, 0, 2, 4, 5, 1, 6, 12, 11, 13, 9, 7, 15, 3, 14, 8 }, Actual = 59, KorfNodesExpanded = 1809933698 },
            new KorfPuzzle { Number = 50, InitialState = new byte[] { 12, 5, 13, 11, 2, 10, 0, 9, 7, 8, 4, 3, 14, 6, 15, 1 }, Actual = 53, KorfNodesExpanded = 63036422 },
            new KorfPuzzle { Number = 51, InitialState = new byte[] { 10, 2, 8, 4, 15, 0, 1, 14, 11, 13, 3, 6, 9, 7, 5, 12 }, Actual = 56, KorfNodesExpanded = 26622863 },
            new KorfPuzzle { Number = 52, InitialState = new byte[] { 10, 8, 0, 12, 3, 7, 6, 2, 1, 14, 4, 11, 15, 13, 9, 5 }, Actual = 56, KorfNodesExpanded = 377141881 },
            new KorfPuzzle { Number = 53, InitialState = new byte[] { 14, 9, 12, 13, 15, 4, 8, 10, 0, 2, 1, 7, 3, 11, 5, 6 }, Actual = 64, KorfNodesExpanded = 465225698 },
            new KorfPuzzle { Number = 54, InitialState = new byte[] { 12, 11, 0, 8, 10, 2, 13, 15, 5, 4, 7, 3, 6, 9, 14, 1 }, Actual = 52, KorfNodesExpanded = 220374385 },
            new KorfPuzzle { Number = 55, InitialState = new byte[] { 13, 8, 14, 3, 9, 1, 0, 7, 15, 5, 4, 10, 12, 2, 6, 11 }, Actual = 41, KorfNodesExpanded = 927212 },
            new KorfPuzzle { Number = 56, InitialState = new byte[] { 3, 15, 2, 5, 11, 6, 4, 7, 12, 9, 1, 0, 13, 14, 10, 8 }, Actual = 55, KorfNodesExpanded = 1199487996 },
            new KorfPuzzle { Number = 57, InitialState = new byte[] { 5, 11, 6, 9, 4, 13, 12, 0, 8, 2, 15, 10, 1, 7, 3, 14 }, Actual = 50, KorfNodesExpanded = 8841527 },
            new KorfPuzzle { Number = 58, InitialState = new byte[] { 5, 0, 15, 8, 4, 6, 1, 14, 10, 11, 3, 9, 7, 12, 2, 13 }, Actual = 51, KorfNodesExpanded = 12955404 },
            new KorfPuzzle { Number = 59, InitialState = new byte[] { 15, 14, 6, 7, 10, 1, 0, 11, 12, 8, 4, 9, 2, 5, 13, 3 }, Actual = 57, KorfNodesExpanded = 1207520464 },
            new KorfPuzzle { Number = 60, InitialState = new byte[] { 11, 14, 13, 1, 2, 3, 12, 4, 15, 7, 9, 5, 10, 6, 8, 0 }, Actual = 66, KorfNodesExpanded = 3337690331 },
            new KorfPuzzle { Number = 61, InitialState = new byte[] { 6, 13, 3, 2, 11, 9, 5, 10, 1, 7, 12, 14, 8, 4, 0, 15 }, Actual = 45, KorfNodesExpanded = 7096850 },
            new KorfPuzzle { Number = 62, InitialState = new byte[] { 4, 6, 12, 0, 14, 2, 9, 13, 11, 8, 3, 15, 7, 10, 1, 5 }, Actual = 57, KorfNodesExpanded = 23540413 },
            new KorfPuzzle { Number = 63, InitialState = new byte[] { 8, 10, 9, 11, 14, 1, 7, 15, 13, 4, 0, 12, 6, 2, 5, 3 }, Actual = 56, KorfNodesExpanded = 995472712 },
            new KorfPuzzle { Number = 64, InitialState = new byte[] { 5, 2, 14, 0, 7, 8, 6, 3, 11, 12, 13, 15, 4, 10, 9, 1 }, Actual = 51, KorfNodesExpanded = 260054152 },
            new KorfPuzzle { Number = 65, InitialState = new byte[] { 7, 8, 3, 2, 10, 12, 4, 6, 11, 13, 5, 15, 0, 1, 9, 14 }, Actual = 47, KorfNodesExpanded = 18997681 },
            new KorfPuzzle { Number = 66, InitialState = new byte[] { 11, 6, 14, 12, 3, 5, 1, 15, 8, 0, 10, 13, 9, 7, 4, 2 }, Actual = 61, KorfNodesExpanded = 1957191378 },
            new KorfPuzzle { Number = 67, InitialState = new byte[] { 7, 1, 2, 4, 8, 3, 6, 11, 10, 15, 0, 5, 14, 12, 13, 9 }, Actual = 50, KorfNodesExpanded = 252783878 },
            new KorfPuzzle { Number = 68, InitialState = new byte[] { 7, 3, 1, 13, 12, 10, 5, 2, 8, 0, 6, 11, 14, 15, 4, 9 }, Actual = 51, KorfNodesExpanded = 64367799 },
            new KorfPuzzle { Number = 69, InitialState = new byte[] { 6, 0, 5, 15, 1, 14, 4, 9, 2, 13, 8, 10, 11, 12, 7, 3 }, Actual = 53, KorfNodesExpanded = 109562359 },
            new KorfPuzzle { Number = 70, InitialState = new byte[] { 15, 1, 3, 12, 4, 0, 6, 5, 2, 8, 14, 9, 13, 10, 7, 11 }, Actual = 52, KorfNodesExpanded = 151042571 },
            new KorfPuzzle { Number = 71, InitialState = new byte[] { 5, 7, 0, 11, 12, 1, 9, 10, 15, 6, 2, 3, 8, 4, 13, 14 }, Actual = 44, KorfNodesExpanded = 8885972 },
            new KorfPuzzle { Number = 72, InitialState = new byte[] { 12, 15, 11, 10, 4, 5, 14, 0, 13, 7, 1, 2, 9, 8, 3, 6 }, Actual = 56, KorfNodesExpanded = 1031641140 },
            new KorfPuzzle { Number = 73, InitialState = new byte[] { 6, 14, 10, 5, 15, 8, 7, 1, 3, 4, 2, 0, 12, 9, 11, 13 }, Actual = 49, KorfNodesExpanded = 3222276 },
            new KorfPuzzle { Number = 74, InitialState = new byte[] { 14, 13, 4, 11, 15, 8, 6, 9, 0, 7, 3, 1, 2, 10, 12, 5 }, Actual = 56, KorfNodesExpanded = 1897728 },
            new KorfPuzzle { Number = 75, InitialState = new byte[] { 14, 4, 0, 10, 6, 5, 1, 3, 9, 2, 13, 15, 12, 7, 8, 11 }, Actual = 48, KorfNodesExpanded = 42772589 },
            new KorfPuzzle { Number = 76, InitialState = new byte[] { 15, 10, 8, 3, 0, 6, 9, 5, 1, 14, 13, 11, 7, 2, 12, 4 }, Actual = 57, KorfNodesExpanded = 126638417 },
            new KorfPuzzle { Number = 77, InitialState = new byte[] { 0, 13, 2, 4, 12, 14, 6, 9, 15, 1, 10, 3, 11, 5, 8, 7 }, Actual = 54, KorfNodesExpanded = 18918269 },
            new KorfPuzzle { Number = 78, InitialState = new byte[] { 3, 14, 13, 6, 4, 15, 8, 9, 5, 12, 10, 0, 2, 7, 1, 11 }, Actual = 53, KorfNodesExpanded = 10907150 },
            new KorfPuzzle { Number = 79, InitialState = new byte[] { 0, 1, 9, 7, 11, 13, 5, 3, 14, 12, 4, 2, 8, 6, 10, 15 }, Actual = 42, KorfNodesExpanded = 540860 },
            new KorfPuzzle { Number = 80, InitialState = new byte[] { 11, 0, 15, 8, 13, 12, 3, 5, 10, 1, 4, 6, 14, 9, 7, 2 }, Actual = 57, KorfNodesExpanded = 132945856 },
            new KorfPuzzle { Number = 81, InitialState = new byte[] { 13, 0, 9, 12, 11, 6, 3, 5, 15, 8, 1, 10, 4, 14, 2, 7 }, Actual = 53, KorfNodesExpanded = 9982569 },
            new KorfPuzzle { Number = 82, InitialState = new byte[] { 14, 10, 2, 1, 13, 9, 8, 11, 7, 3, 6, 12, 15, 5, 4, 0 }, Actual = 62, KorfNodesExpanded = 5506801123 },
            new KorfPuzzle { Number = 83, InitialState = new byte[] { 12, 3, 9, 1, 4, 5, 10, 2, 6, 11, 15, 0, 14, 7, 13, 8 }, Actual = 49, KorfNodesExpanded = 65533432 },
            new KorfPuzzle { Number = 84, InitialState = new byte[] { 15, 8, 10, 7, 0, 12, 14, 1, 5, 9, 6, 3, 13, 11, 4, 2 }, Actual = 55, KorfNodesExpanded = 106074303 },
            new KorfPuzzle { Number = 85, InitialState = new byte[] { 4, 7, 13, 10, 1, 2, 9, 6, 12, 8, 14, 5, 3, 0, 11, 15 }, Actual = 44, KorfNodesExpanded = 2725456 },
            new KorfPuzzle { Number = 86, InitialState = new byte[] { 6, 0, 5, 10, 11, 12, 9, 2, 1, 7, 4, 3, 14, 8, 13, 15 }, Actual = 45, KorfNodesExpanded = 2304426 },
            new KorfPuzzle { Number = 87, InitialState = new byte[] { 9, 5, 11, 10, 13, 0, 2, 1, 8, 6, 14, 12, 4, 7, 3, 15 }, Actual = 52, KorfNodesExpanded = 64926494 },
            new KorfPuzzle { Number = 88, InitialState = new byte[] { 15, 2, 12, 11, 14, 13, 9, 5, 1, 3, 8, 7, 0, 10, 6, 4 }, Actual = 65, KorfNodesExpanded = 6009130748 }, // misprint
            new KorfPuzzle { Number = 89, InitialState = new byte[] { 11, 1, 7, 4, 10, 13, 3, 8, 9, 14, 0, 15, 6, 5, 2, 12 }, Actual = 54, KorfNodesExpanded = 166571097 }, // misprint
            new KorfPuzzle { Number = 90, InitialState = new byte[] { 5, 4, 7, 1, 11, 12, 14, 15, 10, 13, 8, 6, 2, 0, 9, 3 }, Actual = 50, KorfNodesExpanded = 7171137 },
            new KorfPuzzle { Number = 91, InitialState = new byte[] { 9, 7, 5, 2, 14, 15, 12, 10, 11, 3, 6, 1, 8, 13, 0, 4 }, Actual = 57, KorfNodesExpanded = 602886858 },
            new KorfPuzzle { Number = 92, InitialState = new byte[] { 3, 2, 7, 9, 0, 15, 12, 4, 6, 11, 5, 14, 8, 13, 10, 1 }, Actual = 57, KorfNodesExpanded = 1101072541 },
            new KorfPuzzle { Number = 93, InitialState = new byte[] { 13, 9, 14, 6, 12, 8, 1, 2, 3, 4, 0, 7, 5, 10, 11, 15 }, Actual = 46, KorfNodesExpanded = 1599909 },
            new KorfPuzzle { Number = 94, InitialState = new byte[] { 5, 7, 11, 8, 0, 14, 9, 13, 10, 12, 3, 15, 6, 1, 4, 2 }, Actual = 53, KorfNodesExpanded = 1337340 },
            new KorfPuzzle { Number = 95, InitialState = new byte[] { 4, 3, 6, 13, 7, 15, 9, 0, 10, 5, 8, 11, 2, 12, 1, 14 }, Actual = 50, KorfNodesExpanded = 7115967 },
            new KorfPuzzle { Number = 96, InitialState = new byte[] { 1, 7, 15, 14, 2, 6, 4, 9, 12, 11, 13, 3, 0, 8, 5, 10 }, Actual = 49, KorfNodesExpanded = 12808564 },
            new KorfPuzzle { Number = 97, InitialState = new byte[] { 9, 14, 5, 7, 8, 15, 1, 2, 10, 4, 13, 6, 12, 0, 11, 3 }, Actual = 44, KorfNodesExpanded = 1002927 },
            new KorfPuzzle { Number = 98, InitialState = new byte[] { 0, 11, 3, 12, 5, 2, 1, 9, 8, 10, 14, 15, 7, 4, 13, 6 }, Actual = 54, KorfNodesExpanded = 183526883 },
            new KorfPuzzle { Number = 99, InitialState = new byte[] { 7, 15, 4, 0, 10, 9, 2, 5, 12, 11, 13, 6, 1, 3, 14, 8 }, Actual = 57, KorfNodesExpanded = 83477694 },
            new KorfPuzzle { Number = 100, InitialState = new byte[] { 11, 4, 0, 8, 6, 10, 5, 13, 12, 7, 14, 3, 1, 2, 9, 15 }, Actual = 54, KorfNodesExpanded = 67880056 },
        };
    }

    class KorfPuzzle
    {
        public int Number { get; set; }
        public byte[] InitialState { get; set; }
        public int Actual { get; set; }
        public ulong KorfNodesExpanded { get; set; }

        public byte[] Goal
        {
            get { return Enumerable.Range(0, InitialState.Length).Select(Convert.ToByte).ToArray(); }
        }
    }
}
