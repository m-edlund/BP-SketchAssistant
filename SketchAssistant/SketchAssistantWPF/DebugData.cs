﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SketchAssistantWPF
{
    public class DebugData
    {
        Random rnd;

        public DebugData(){ rnd = new Random(); }

        public Point[] debugPoints1 = { new Point(78, 81), new Point(78, 82), new Point(79, 82), new Point(80, 83), new Point(80, 84), new Point(81, 84), new Point(81, 85), new Point(82, 86), new Point(83, 87), new Point(84, 87), new Point(84, 88), new Point(85, 89), new Point(85, 90), new Point(86, 90), new Point(87, 91), new Point(87, 92), new Point(88, 92), new Point(88, 93), new Point(89, 94), new Point(90, 95), new Point(90, 96), new Point(91, 96), new Point(92, 97), new Point(93, 98), new Point(93, 99), new Point(94, 99), new Point(94, 100), new Point(95, 101), new Point(96, 102), new Point(97, 103), new Point(98, 104), new Point(98, 105), new Point(99, 105), new Point(99, 106), new Point(100, 107), new Point(101, 108), new Point(102, 108), new Point(102, 109), new Point(103, 109), new Point(103, 110), new Point(104, 110), new Point(104, 111), new Point(105, 112), new Point(105, 113), new Point(106, 113), new Point(107, 114), new Point(108, 115), new Point(108, 116), new Point(109, 116), new Point(109, 117), new Point(109, 118), new Point(110, 118), new Point(111, 119), new Point(111, 120), new Point(112, 120), new Point(112, 121), new Point(113, 122), new Point(113, 123), new Point(114, 123), new Point(114, 124), new Point(115, 124), new Point(115, 125), new Point(116, 126), new Point(117, 127), new Point(118, 128), new Point(119, 129), new Point(119, 130), new Point(120, 130), new Point(120, 131), new Point(121, 132), new Point(122, 133), new Point(123, 134), new Point(123, 135), new Point(124, 135), new Point(124, 136), new Point(125, 136), new Point(125, 137), new Point(126, 137), new Point(126, 138), new Point(127, 138), new Point(127, 139), new Point(128, 139), new Point(128, 140), new Point(128, 141), new Point(129, 141), new Point(129, 142), new Point(130, 142), new Point(130, 143), new Point(131, 144), new Point(131, 145), new Point(132, 145), new Point(132, 146), new Point(133, 146), new Point(133, 147), new Point(134, 148), new Point(134, 149), new Point(135, 149), new Point(135, 150), new Point(136, 151), new Point(137, 152), new Point(138, 153), new Point(138, 154), new Point(139, 154), new Point(139, 155), new Point(140, 156), new Point(141, 156), new Point(141, 157), new Point(142, 157), new Point(143, 158), new Point(143, 159), new Point(144, 159), new Point(145, 160), new Point(146, 160), new Point(146, 161), new Point(147, 161), new Point(147, 162), new Point(148, 162), new Point(149, 162), new Point(149, 163), new Point(150, 163), new Point(150, 164), new Point(151, 164), new Point(151, 165), new Point(152, 165), new Point(153, 166), new Point(154, 167), new Point(155, 167), new Point(155, 168), new Point(156, 169), new Point(157, 170), new Point(158, 171), new Point(158, 172), new Point(159, 172), new Point(159, 173), new Point(160, 173), new Point(160, 174), new Point(161, 175), new Point(162, 176), new Point(163, 177), new Point(163, 178), new Point(164, 178), new Point(165, 179), new Point(165, 180), new Point(166, 181), new Point(167, 181), new Point(168, 182), new Point(168, 183), new Point(169, 184), new Point(170, 184), new Point(170, 185), new Point(171, 186), new Point(172, 186), new Point(172, 187), new Point(173, 188), new Point(174, 188), new Point(174, 189), new Point(175, 190), new Point(176, 191), new Point(177, 192), new Point(178, 192), new Point(178, 193), new Point(179, 194), new Point(180, 195), new Point(181, 195), new Point(181, 196), new Point(182, 196), new Point(182, 197), new Point(183, 198), new Point(184, 198), new Point(184, 199), new Point(185, 200), new Point(186, 201), new Point(187, 202), new Point(188, 203), new Point(189, 203), new Point(190, 204), new Point(191, 205), new Point(191, 206), new Point(192, 207), new Point(193, 208), new Point(194, 209), new Point(194, 210), new Point(195, 210), new Point(195, 211), new Point(196, 212), new Point(197, 212), new Point(197, 213), new Point(198, 214), new Point(198, 215), new Point(199, 215), new Point(200, 216), new Point(200, 217), new Point(201, 218), new Point(202, 219), new Point(203, 220), new Point(204, 221), new Point(204, 222), new Point(205, 222), new Point(205, 223), new Point(206, 224), new Point(207, 225), new Point(207, 226), new Point(208, 227), new Point(209, 227), new Point(209, 228), new Point(210, 229), new Point(211, 230), new Point(211, 231), new Point(212, 231), new Point(213, 232), new Point(214, 233), new Point(214, 234), new Point(215, 235), new Point(216, 235), new Point(217, 236), new Point(217, 237), new Point(218, 238), new Point(219, 239), new Point(219, 240), new Point(220, 241), new Point(221, 242), new Point(222, 243), new Point(223, 244), new Point(223, 245), new Point(224, 246), new Point(225, 247), new Point(225, 248), new Point(226, 248), new Point(226, 249), new Point(227, 250), new Point(227, 251), new Point(228, 251), new Point(229, 252), new Point(229, 253), new Point(230, 254), new Point(230, 255), new Point(231, 256), new Point(232, 257), new Point(233, 258), new Point(233, 259), new Point(234, 260), new Point(235, 261), new Point(235, 262), new Point(236, 263), new Point(237, 264), new Point(238, 265), new Point(239, 266), new Point(240, 267), new Point(240, 268), new Point(241, 269), new Point(242, 270), new Point(242, 271), new Point(243, 272), new Point(244, 273), new Point(244, 274), new Point(245, 275), new Point(246, 275), new Point(246, 276), new Point(247, 277), new Point(248, 278), new Point(249, 279), new Point(249, 280), new Point(250, 281), new Point(251, 282), new Point(251, 283), new Point(252, 284), new Point(252, 285), new Point(253, 285), new Point(254, 287), new Point(255, 288), new Point(256, 289), new Point(256, 290), new Point(257, 290), new Point(258, 291), new Point(258, 292), new Point(259, 293), new Point(260, 294), new Point(260, 295), new Point(261, 295), new Point(261, 296), new Point(262, 296), new Point(262, 297), new Point(263, 297), new Point(263, 298), new Point(263, 299), new Point(264, 299), new Point(264, 300), new Point(265, 300), new Point(265, 301), new Point(266, 301), new Point(266, 302), new Point(267, 303), new Point(268, 304), new Point(269, 305), new Point(270, 306), new Point(271, 307), new Point(271, 308), new Point(272, 308), new Point(272, 309), new Point(273, 309), new Point(273, 310), new Point(274, 310), new Point(274, 311), new Point(275, 311), new Point(275, 312), new Point(276, 312), new Point(276, 313), new Point(277, 314), new Point(277, 315), new Point(278, 315), new Point(278, 316), new Point(279, 316), new Point(279, 317), new Point(280, 317), new Point(281, 318), new Point(281, 319) };
        public Point[] debugPoints2 = { new Point(72, 50), new Point(72, 51), new Point(72, 52), new Point(72, 53), new Point(73, 53), new Point(74, 53), new Point(75, 53), new Point(76, 53), new Point(77, 53), new Point(77, 54), new Point(78, 54), new Point(79, 54), new Point(80, 54), new Point(81, 54), new Point(82, 54), new Point(83, 54), new Point(84, 54), new Point(85, 54), new Point(86, 54), new Point(87, 54), new Point(88, 54), new Point(89, 54), new Point(90, 54), new Point(91, 54), new Point(92, 54), new Point(93, 54), new Point(94, 54), new Point(95, 54), new Point(96, 54), new Point(97, 54), new Point(98, 54), new Point(99, 54), new Point(100, 54), new Point(101, 54), new Point(102, 54), new Point(103, 54), new Point(104, 54), new Point(105, 54), new Point(106, 54), new Point(107, 54), new Point(109, 54), new Point(110, 54), new Point(111, 54), new Point(112, 54), new Point(113, 54), new Point(114, 54), new Point(115, 54), new Point(116, 54), new Point(117, 54), new Point(118, 54), new Point(119, 54), new Point(120, 54), new Point(121, 54), new Point(122, 54), new Point(123, 54), new Point(124, 54), new Point(125, 54), new Point(126, 54), new Point(127, 54), new Point(128, 54), new Point(129, 54), new Point(130, 54), new Point(131, 54), new Point(132, 54), new Point(133, 54), new Point(134, 54), new Point(135, 54), new Point(136, 54), new Point(137, 54), new Point(138, 54), new Point(139, 54), new Point(140, 54), new Point(141, 54), new Point(142, 54), new Point(143, 54), new Point(144, 54), new Point(145, 54), new Point(146, 54), new Point(147, 54), new Point(148, 54), new Point(149, 54), new Point(150, 54), new Point(151, 54), new Point(152, 54), new Point(154, 54), new Point(155, 54), new Point(156, 54), new Point(157, 54), new Point(158, 54), new Point(160, 54), new Point(161, 54), new Point(162, 54), new Point(164, 54), new Point(165, 54), new Point(167, 54), new Point(168, 54), new Point(170, 54), new Point(171, 54), new Point(173, 54), new Point(174, 54), new Point(176, 54), new Point(178, 54), new Point(180, 54), new Point(181, 54), new Point(183, 54), new Point(185, 54), new Point(187, 54), new Point(188, 54), new Point(190, 54), new Point(192, 54), new Point(194, 54), new Point(196, 54), new Point(198, 54), new Point(200, 54), new Point(202, 54), new Point(203, 54), new Point(205, 54), new Point(207, 54), new Point(209, 54), new Point(210, 54), new Point(212, 54), new Point(214, 54), new Point(215, 54), new Point(217, 54), new Point(219, 54), new Point(220, 54), new Point(222, 54), new Point(224, 54), new Point(225, 54), new Point(227, 54), new Point(228, 54), new Point(230, 54), new Point(231, 54), new Point(233, 54), new Point(234, 54), new Point(235, 54), new Point(237, 54), new Point(238, 54), new Point(239, 54), new Point(240, 54), new Point(242, 54), new Point(243, 54), new Point(244, 54), new Point(245, 54), new Point(246, 54), new Point(248, 54), new Point(249, 54), new Point(250, 54), new Point(251, 54), new Point(253, 54), new Point(254, 54), new Point(255, 54), new Point(256, 54), new Point(257, 54), new Point(258, 54), new Point(259, 54), new Point(260, 54), new Point(262, 54), new Point(263, 54), new Point(264, 54), new Point(265, 54), new Point(266, 54), new Point(267, 54), new Point(268, 54), new Point(269, 54), new Point(270, 54), new Point(271, 54), new Point(272, 54), new Point(273, 54), new Point(274, 54), new Point(275, 54), new Point(276, 54), new Point(277, 54), new Point(278, 54), new Point(279, 54), new Point(280, 54), new Point(281, 54), new Point(282, 54), new Point(283, 54), new Point(284, 54), new Point(285, 54), new Point(286, 54), new Point(287, 54), new Point(288, 54), new Point(289, 54), new Point(290, 54), new Point(291, 54), new Point(292, 54), new Point(293, 54), new Point(294, 55), new Point(295, 55), new Point(296, 55), new Point(297, 55), new Point(298, 55), new Point(299, 55), new Point(300, 56), new Point(301, 56), new Point(302, 57), new Point(303, 57), new Point(304, 57), new Point(305, 57), new Point(306, 58), new Point(307, 58), new Point(308, 58), new Point(308, 59), new Point(309, 59), new Point(310, 60), new Point(311, 60), new Point(311, 61), new Point(312, 61), new Point(313, 61), new Point(313, 62), new Point(314, 62), new Point(314, 63), new Point(315, 63), new Point(315, 64), new Point(316, 64), new Point(316, 65), new Point(317, 65), new Point(317, 66), new Point(318, 66), new Point(318, 67), new Point(318, 68), new Point(319, 69), new Point(320, 70), new Point(321, 71), new Point(321, 72), new Point(322, 73), new Point(322, 74), new Point(323, 75), new Point(324, 76), new Point(324, 78), new Point(325, 79), new Point(326, 80), new Point(326, 81), new Point(327, 83), new Point(328, 84), new Point(328, 85), new Point(329, 87), new Point(330, 88), new Point(330, 89), new Point(331, 91), new Point(332, 92), new Point(332, 93), new Point(333, 94), new Point(334, 95), new Point(334, 96), new Point(335, 98), new Point(335, 99), new Point(336, 100), new Point(337, 101), new Point(337, 102), new Point(338, 103), new Point(339, 104), new Point(339, 105), new Point(340, 106), new Point(340, 107), new Point(340, 108), new Point(341, 109), new Point(341, 110), new Point(342, 111), new Point(342, 112), new Point(343, 113), new Point(343, 114), new Point(343, 115), new Point(344, 116), new Point(344, 117), new Point(344, 118), new Point(344, 119), new Point(344, 120), new Point(345, 121), new Point(345, 122), new Point(345, 123), new Point(345, 124), new Point(345, 125), new Point(345, 127), new Point(345, 128), new Point(345, 129), new Point(345, 130), new Point(345, 131), new Point(345, 132), new Point(345, 133), new Point(345, 134), new Point(345, 135), new Point(345, 136), new Point(345, 137), new Point(345, 138), new Point(345, 139), new Point(345, 140), new Point(345, 141), new Point(345, 142), new Point(345, 143), new Point(345, 144), new Point(345, 145), new Point(345, 146), new Point(345, 147), new Point(345, 148), new Point(345, 149), new Point(345, 150), new Point(345, 151), new Point(345, 152), new Point(345, 153), new Point(344, 154), new Point(344, 155), new Point(343, 156), new Point(342, 157), new Point(342, 158), new Point(341, 158), new Point(341, 159), new Point(340, 160), new Point(340, 161), new Point(339, 162), new Point(339, 163), new Point(338, 163), new Point(337, 164), new Point(337, 165), new Point(336, 166), new Point(335, 167), new Point(335, 168), new Point(334, 168), new Point(333, 169), new Point(333, 170), new Point(332, 171), new Point(331, 172), new Point(331, 173), new Point(330, 174), new Point(329, 175), new Point(328, 176), new Point(328, 177), new Point(327, 178), new Point(326, 178), new Point(325, 180), new Point(324, 181), new Point(323, 182), new Point(322, 183), new Point(322, 184), new Point(321, 185), new Point(320, 186), new Point(319, 187), new Point(318, 187), new Point(317, 188), new Point(316, 189), new Point(315, 190), new Point(314, 191), new Point(313, 192), new Point(312, 193), new Point(311, 194), new Point(310, 195), new Point(309, 196), new Point(308, 197), new Point(307, 198), new Point(306, 199), new Point(305, 200), new Point(304, 201), new Point(303, 202), new Point(302, 203), new Point(301, 204), new Point(300, 205), new Point(299, 206), new Point(298, 207), new Point(297, 208), new Point(295, 208), new Point(295, 209), new Point(293, 210), new Point(292, 211), new Point(291, 212), new Point(291, 213), new Point(290, 214), new Point(288, 215), new Point(287, 216), new Point(286, 217), new Point(285, 218), new Point(284, 219), new Point(282, 220), new Point(281, 221), new Point(280, 222), new Point(279, 222), new Point(278, 223), new Point(276, 224), new Point(275, 225), new Point(274, 226), new Point(273, 227), new Point(272, 228), new Point(270, 229), new Point(269, 230), new Point(268, 231), new Point(267, 232), new Point(266, 232), new Point(265, 233), new Point(263, 234), new Point(262, 235), new Point(261, 236), new Point(260, 237), new Point(259, 238), new Point(258, 238), new Point(257, 239), new Point(256, 240), new Point(255, 241), new Point(254, 242), new Point(253, 243), new Point(252, 244), new Point(251, 245), new Point(250, 246), new Point(249, 247), new Point(248, 248), new Point(247, 249), new Point(246, 250), new Point(245, 251), new Point(243, 252), new Point(242, 253), new Point(241, 255), new Point(240, 256), new Point(239, 257), new Point(237, 258), new Point(236, 259), new Point(235, 260), new Point(234, 261), new Point(233, 262), new Point(232, 263), new Point(230, 264), new Point(229, 265), new Point(228, 266), new Point(227, 267), new Point(226, 268), new Point(225, 269), new Point(223, 270), new Point(222, 271), new Point(221, 272), new Point(220, 273), new Point(219, 274), new Point(218, 275), new Point(217, 276), new Point(216, 277), new Point(215, 278), new Point(214, 279), new Point(213, 280), new Point(211, 281), new Point(210, 282), new Point(209, 283), new Point(208, 284), new Point(207, 285), new Point(206, 286), new Point(205, 287), new Point(204, 288), new Point(203, 288), new Point(201, 290), new Point(200, 291), new Point(199, 292), new Point(198, 293), new Point(197, 294), new Point(196, 295), new Point(195, 296), new Point(194, 296), new Point(193, 297), new Point(192, 298), new Point(191, 299), new Point(190, 300), new Point(189, 301), new Point(188, 301), new Point(187, 302), new Point(186, 303), new Point(185, 304), new Point(184, 305), new Point(183, 306), new Point(182, 307), new Point(181, 307), new Point(180, 308), new Point(179, 309), new Point(178, 309), new Point(177, 310), new Point(176, 311), new Point(175, 311), new Point(174, 312), new Point(173, 313), new Point(172, 313), new Point(171, 314), new Point(170, 314), new Point(169, 315), new Point(168, 315), new Point(167, 316), new Point(166, 316), new Point(165, 317), new Point(164, 318), new Point(163, 318), new Point(162, 319), new Point(161, 319), new Point(160, 320), new Point(159, 321), new Point(158, 321), new Point(157, 322), new Point(156, 322), new Point(155, 323), new Point(154, 323), new Point(153, 324), new Point(152, 324), new Point(151, 325), new Point(150, 326), new Point(149, 326), new Point(148, 327), new Point(147, 327), new Point(146, 328), new Point(145, 329), new Point(144, 330), new Point(143, 330), new Point(143, 331), new Point(142, 331), new Point(141, 332), new Point(140, 332), new Point(139, 333), new Point(138, 334), new Point(137, 335), new Point(136, 335), new Point(135, 336), new Point(135, 337), new Point(134, 337), new Point(133, 338), new Point(132, 338), new Point(132, 339), new Point(131, 340), new Point(130, 340), new Point(129, 341), new Point(128, 341), new Point(128, 342), new Point(127, 343), new Point(126, 343), new Point(125, 344), new Point(125, 345), new Point(124, 345), new Point(123, 346), new Point(122, 347), new Point(122, 348), new Point(121, 348), new Point(120, 349), new Point(120, 350), new Point(119, 350), new Point(119, 351), new Point(118, 352), new Point(117, 353), new Point(116, 353), new Point(116, 354), new Point(115, 354), new Point(115, 355), new Point(114, 356), new Point(113, 356), new Point(112, 357), new Point(111, 357), new Point(111, 358), new Point(110, 359), new Point(109, 360), new Point(108, 360), new Point(108, 361), new Point(107, 362), new Point(106, 363), new Point(106, 364), new Point(105, 365), new Point(104, 366), new Point(104, 367), new Point(103, 368), new Point(103, 369), new Point(102, 370), new Point(102, 371), new Point(101, 371), new Point(101, 372), new Point(100, 373), new Point(100, 374), new Point(99, 375), new Point(99, 376), new Point(98, 377), new Point(98, 378), new Point(97, 379), new Point(97, 380), new Point(97, 381), new Point(96, 382), new Point(96, 383), new Point(96, 384), new Point(96, 385), new Point(96, 386), new Point(96, 387), new Point(96, 388), new Point(96, 389), new Point(96, 390), new Point(96, 391), new Point(96, 392), new Point(96, 393), new Point(96, 394), new Point(96, 395), new Point(96, 396), new Point(96, 397), new Point(96, 398), new Point(96, 399), new Point(96, 400), new Point(96, 401), new Point(96, 402), new Point(96, 403), new Point(96, 404), new Point(96, 405), new Point(97, 405), new Point(97, 406), new Point(98, 407), new Point(99, 408), new Point(99, 409), new Point(100, 409), new Point(101, 410), new Point(102, 410), new Point(102, 411), new Point(103, 411), new Point(104, 412), new Point(105, 413), new Point(106, 413), new Point(106, 414), new Point(107, 415), new Point(108, 415), new Point(109, 416), new Point(110, 417), new Point(111, 417), new Point(112, 418), new Point(114, 419), new Point(115, 419), new Point(116, 420), new Point(117, 421), new Point(118, 422), new Point(119, 422), new Point(120, 423), new Point(121, 424), new Point(122, 424), new Point(123, 425), new Point(124, 426), new Point(125, 426), new Point(126, 427), new Point(127, 428), new Point(128, 428), new Point(129, 429), new Point(130, 430), new Point(132, 430), new Point(133, 431), new Point(134, 432), new Point(135, 432), new Point(136, 433), new Point(138, 434), new Point(139, 434), new Point(140, 435), new Point(141, 435), new Point(142, 436), new Point(143, 436), new Point(144, 437), new Point(145, 437), new Point(147, 438), new Point(148, 439), new Point(149, 439), new Point(150, 440), new Point(151, 441), new Point(153, 441), new Point(154, 442), new Point(155, 442), new Point(157, 443), new Point(158, 444), new Point(159, 444), new Point(160, 445), new Point(161, 445), new Point(162, 445), new Point(163, 446), new Point(165, 446), new Point(166, 446), new Point(167, 447), new Point(168, 447), new Point(170, 447), new Point(171, 447), new Point(172, 447), new Point(173, 447), new Point(175, 447), new Point(176, 447), new Point(177, 447), new Point(178, 447), new Point(179, 447), new Point(181, 447), new Point(182, 448), new Point(183, 448), new Point(185, 448), new Point(186, 448), new Point(188, 448), new Point(189, 448), new Point(190, 448), new Point(192, 448), new Point(194, 448), new Point(195, 448), new Point(197, 448), new Point(198, 448), new Point(200, 448), new Point(202, 448), new Point(203, 448), new Point(205, 448), new Point(207, 448), new Point(209, 448), new Point(210, 448), new Point(212, 448), new Point(214, 448), new Point(216, 448), new Point(217, 448), new Point(219, 448), new Point(221, 448), new Point(223, 448), new Point(225, 448), new Point(226, 448), new Point(228, 448), new Point(230, 448), new Point(232, 448), new Point(234, 448), new Point(235, 448), new Point(237, 448), new Point(239, 448), new Point(241, 448), new Point(243, 448), new Point(244, 448), new Point(246, 448), new Point(248, 448), new Point(250, 448), new Point(251, 448), new Point(253, 448), new Point(255, 448), new Point(256, 448), new Point(258, 448), new Point(259, 448), new Point(261, 448), new Point(263, 448), new Point(264, 448), new Point(266, 448), new Point(267, 448), new Point(269, 448), new Point(270, 448), new Point(272, 448), new Point(273, 448), new Point(275, 448), new Point(276, 448), new Point(277, 448), new Point(279, 448), new Point(280, 448), new Point(282, 448), new Point(283, 448), new Point(285, 448), new Point(286, 448), new Point(288, 448), new Point(289, 448), new Point(290, 448), new Point(292, 448), new Point(293, 448), new Point(294, 448), new Point(296, 448), new Point(297, 448), new Point(298, 448), new Point(299, 448), new Point(301, 448), new Point(302, 448), new Point(303, 448), new Point(304, 448), new Point(305, 448), new Point(306, 448), new Point(307, 448), new Point(308, 448), new Point(309, 448), new Point(310, 448), new Point(311, 448), new Point(312, 448), new Point(313, 448), new Point(314, 448), new Point(315, 448), new Point(316, 448), new Point(316, 447), new Point(317, 447), new Point(318, 447), new Point(319, 447), new Point(319, 446), new Point(320, 446), new Point(321, 446), new Point(322, 445), new Point(323, 445), new Point(324, 444), new Point(325, 444), new Point(326, 444), new Point(326, 443), new Point(327, 443), new Point(328, 442), new Point(329, 442), new Point(329, 441), new Point(330, 441), new Point(330, 440), new Point(331, 440), new Point(331, 439), new Point(332, 439), new Point(332, 438), new Point(333, 438), new Point(333, 437), new Point(334, 437), new Point(334, 436), new Point(335, 436), new Point(335, 435), new Point(336, 435), new Point(336, 434), new Point(337, 434), new Point(337, 433), new Point(338, 433), new Point(338, 432), new Point(339, 431), new Point(340, 430), new Point(340, 429), new Point(341, 429), new Point(341, 428), new Point(342, 427), new Point(342, 426), new Point(343, 425), new Point(343, 424), new Point(343, 423), new Point(343, 422), new Point(344, 421), new Point(344, 420), new Point(344, 419), new Point(344, 418), new Point(344, 417), new Point(344, 416), new Point(344, 415), new Point(344, 414), new Point(344, 412), new Point(344, 411), new Point(344, 410), new Point(344, 408), new Point(344, 407), new Point(344, 406), new Point(344, 404), new Point(344, 402), new Point(343, 401), new Point(343, 399), new Point(342, 397), new Point(341, 396), new Point(340, 394), new Point(339, 392), new Point(338, 391), new Point(337, 389), new Point(336, 387), new Point(335, 385), new Point(334, 383), new Point(333, 381), new Point(332, 380), new Point(330, 378), new Point(329, 376), new Point(328, 375), new Point(327, 373), new Point(326, 372), new Point(325, 370), new Point(324, 369), new Point(323, 367), new Point(322, 366), new Point(321, 364), new Point(320, 363), new Point(319, 361), new Point(318, 360), new Point(318, 359), new Point(317, 358), new Point(316, 356), new Point(315, 355), new Point(315, 354), new Point(314, 353), new Point(313, 352), new Point(312, 351), new Point(311, 350), new Point(311, 349), new Point(310, 348), new Point(309, 347), new Point(309, 346), new Point(308, 345), new Point(307, 344), new Point(306, 343), new Point(306, 342), new Point(305, 341), new Point(304, 340), new Point(303, 339), new Point(302, 338), new Point(301, 337), new Point(301, 336), new Point(297, 333), new Point(296, 332), new Point(295, 331), new Point(294, 330), new Point(293, 329), new Point(292, 328), new Point(291, 327), new Point(290, 326), new Point(288, 326), new Point(287, 325), new Point(286, 324), new Point(285, 323), new Point(283, 322), new Point(282, 321), new Point(281, 320), new Point(280, 319), new Point(279, 318), new Point(277, 317), new Point(276, 316), new Point(275, 316), new Point(274, 315), new Point(273, 314), new Point(272, 313), new Point(271, 313), new Point(270, 312), new Point(269, 311), new Point(268, 311), new Point(267, 310), new Point(266, 309), new Point(265, 309), new Point(264, 308), new Point(263, 307), new Point(262, 306), new Point(261, 306), new Point(260, 305), new Point(259, 304), new Point(258, 304), new Point(257, 303), new Point(256, 302), new Point(255, 302), new Point(254, 301), new Point(253, 300), new Point(251, 300), new Point(250, 299), new Point(249, 298), new Point(248, 298), new Point(247, 297), new Point(246, 296), new Point(244, 295), new Point(243, 294), new Point(241, 294), new Point(240, 293), new Point(238, 292), new Point(237, 291), new Point(235, 291), new Point(233, 290), new Point(232, 289), new Point(230, 288), new Point(228, 287), new Point(226, 286), new Point(224, 285), new Point(222, 284), new Point(220, 283), new Point(217, 282), new Point(215, 280), new Point(213, 279), new Point(211, 278), new Point(209, 277), new Point(207, 276), new Point(205, 274), new Point(203, 273), new Point(201, 272), new Point(199, 271), new Point(197, 270), new Point(195, 269), new Point(194, 268), new Point(192, 267), new Point(190, 266), new Point(189, 265), new Point(187, 264), new Point(186, 263), new Point(184, 262), new Point(183, 262), new Point(182, 261), new Point(180, 260), new Point(179, 259), new Point(177, 259), new Point(176, 258), new Point(175, 257), new Point(173, 256), new Point(172, 255), new Point(171, 255), new Point(170, 254), new Point(169, 253), new Point(167, 252), new Point(166, 251), new Point(165, 251), new Point(164, 250), new Point(163, 249), new Point(162, 248), new Point(161, 247), new Point(160, 247), new Point(159, 246), new Point(158, 246), new Point(157, 245), new Point(157, 244), new Point(156, 244), new Point(155, 243), new Point(154, 242), new Point(153, 241), new Point(152, 241), new Point(151, 240), new Point(150, 239), new Point(149, 238), new Point(148, 238), new Point(147, 237), new Point(146, 236), new Point(146, 235), new Point(145, 235), new Point(144, 234), new Point(143, 233), new Point(142, 232), new Point(141, 232), new Point(140, 231), new Point(139, 230), new Point(138, 229), new Point(137, 229), new Point(136, 228), new Point(135, 227), new Point(134, 226), new Point(133, 225), new Point(132, 224), new Point(131, 223), new Point(130, 222), new Point(129, 221), new Point(128, 220), new Point(128, 219), new Point(127, 219), new Point(126, 218), new Point(125, 217), new Point(125, 216), new Point(124, 215), new Point(123, 214), new Point(122, 213), new Point(122, 212), new Point(121, 212), new Point(120, 211), new Point(119, 210), new Point(118, 209), new Point(118, 208), new Point(117, 207), new Point(116, 206), new Point(115, 205), new Point(114, 204), new Point(113, 203), new Point(113, 202), new Point(112, 201), new Point(111, 200), new Point(110, 199), new Point(110, 198), new Point(109, 197), new Point(108, 196), new Point(107, 195), new Point(107, 194), new Point(106, 193), new Point(105, 192), new Point(104, 191), new Point(103, 190), new Point(102, 189), new Point(102, 188), new Point(101, 187), new Point(100, 186), new Point(99, 185), new Point(98, 183), new Point(97, 182), new Point(94, 178), new Point(93, 176), new Point(92, 175), new Point(92, 174), new Point(91, 173), new Point(90, 172), new Point(90, 171), new Point(89, 170), new Point(88, 169), new Point(87, 168), new Point(86, 167), new Point(86, 166), new Point(85, 165), new Point(84, 164), new Point(84, 163), new Point(83, 162), new Point(82, 161), new Point(82, 160), new Point(81, 159), new Point(80, 158), new Point(80, 157), new Point(79, 157), new Point(79, 156), new Point(78, 155), new Point(78, 154), new Point(77, 154), new Point(77, 153), new Point(76, 152), new Point(76, 151), new Point(75, 151), new Point(75, 150), new Point(74, 149), new Point(73, 148), new Point(72, 147), new Point(72, 146), new Point(71, 146), new Point(71, 145), new Point(70, 144), new Point(70, 143), new Point(69, 142), new Point(69, 141), new Point(68, 140), new Point(68, 139), new Point(67, 139), new Point(67, 138), new Point(66, 137), new Point(65, 136), new Point(65, 135), new Point(65, 134), new Point(64, 133), new Point(63, 132), new Point(63, 131), new Point(62, 130), new Point(62, 129), new Point(61, 128), new Point(61, 127), new Point(60, 126), new Point(60, 125), new Point(59, 124), new Point(59, 123), new Point(59, 122), new Point(58, 122), new Point(58, 121), new Point(58, 120), new Point(58, 119), new Point(58, 118), new Point(58, 117), new Point(58, 116), new Point(58, 115), new Point(58, 114), new Point(58, 113), new Point(58, 112), new Point(58, 111), new Point(58, 110), new Point(59, 110), new Point(59, 109), new Point(60, 108), new Point(60, 107), new Point(61, 107), new Point(61, 106), new Point(62, 105), new Point(62, 104), new Point(63, 104), new Point(63, 103), new Point(64, 103), new Point(65, 102), new Point(65, 101), new Point(66, 101), new Point(66, 100), new Point(67, 99), new Point(68, 99), new Point(69, 98), new Point(70, 98), new Point(71, 97), new Point(71, 96), new Point(72, 95), new Point(73, 94), new Point(74, 94), new Point(75, 93), new Point(76, 92), new Point(77, 91), new Point(78, 90), new Point(79, 90), new Point(80, 89), new Point(81, 88), new Point(82, 87), new Point(83, 86), new Point(85, 86), new Point(86, 85), new Point(87, 84), new Point(88, 83), new Point(89, 83), new Point(90, 82), new Point(91, 81), new Point(93, 81), new Point(94, 80), new Point(95, 79), new Point(96, 79), new Point(97, 78), new Point(98, 77), new Point(99, 77), new Point(100, 76), new Point(101, 75), new Point(102, 75), new Point(103, 75), new Point(104, 74), new Point(105, 73), new Point(106, 73), new Point(107, 72), new Point(108, 71), new Point(109, 71), new Point(109, 70), new Point(110, 70), new Point(111, 69), new Point(112, 69), new Point(112, 68), new Point(113, 68), new Point(113, 67), new Point(113, 66), new Point(113, 65), new Point(113, 64), new Point(113, 62), new Point(113, 59), new Point(113, 57), new Point(113, 54), new Point(113, 50), new Point(113, 46), new Point(113, 43), new Point(114, 38), };
        public Point[] debugPoints3 = { new Point(284, 148), new Point(284, 149), new Point(283, 149), new Point(282, 150), new Point(282, 151), new Point(281, 151), new Point(281, 152), new Point(280, 152), new Point(280, 153), new Point(279, 153), new Point(279, 154), new Point(278, 154), new Point(278, 155), new Point(277, 155), new Point(277, 156), new Point(276, 156), new Point(276, 157), new Point(275, 157), new Point(274, 158), new Point(274, 159), new Point(273, 159), new Point(272, 160), new Point(271, 161), new Point(270, 161), new Point(270, 162), new Point(269, 163), new Point(268, 163), new Point(267, 164), new Point(266, 164), new Point(266, 165), new Point(265, 166), new Point(264, 166), new Point(263, 167), new Point(261, 168), new Point(260, 169), new Point(259, 170), new Point(258, 170), new Point(257, 171), new Point(256, 172), new Point(255, 172), new Point(253, 174), new Point(252, 174), new Point(250, 175), new Point(249, 176), new Point(248, 177), new Point(246, 178), new Point(245, 179), new Point(243, 180), new Point(241, 181), new Point(240, 181), new Point(238, 182), new Point(237, 183), new Point(236, 183), new Point(234, 184), new Point(233, 185), new Point(232, 185), new Point(231, 186), new Point(230, 187), new Point(228, 187), new Point(228, 188), new Point(227, 188), new Point(226, 188), new Point(226, 189), new Point(225, 189), new Point(224, 189), new Point(224, 190), new Point(223, 190), new Point(222, 190), new Point(222, 191), new Point(221, 191), new Point(220, 191), new Point(219, 191), new Point(219, 192), new Point(218, 192), new Point(218, 193), new Point(217, 193), new Point(217, 194), new Point(215, 194), new Point(215, 195), new Point(213, 195), new Point(212, 196), new Point(211, 197), new Point(209, 198), new Point(207, 200), new Point(206, 200), new Point(203, 201), new Point(201, 203), new Point(200, 203), new Point(198, 205), new Point(195, 206), new Point(194, 207), new Point(192, 208), new Point(190, 209), new Point(189, 210), new Point(188, 210), new Point(186, 211), new Point(185, 212), new Point(184, 212), new Point(183, 213), new Point(182, 214), new Point(181, 214), new Point(179, 215), new Point(178, 216), new Point(177, 216), new Point(176, 216), new Point(175, 217), new Point(174, 217), new Point(173, 218), new Point(172, 219), new Point(170, 219), new Point(169, 220), new Point(168, 221), new Point(165, 222), new Point(164, 222), new Point(163, 223), new Point(161, 224), new Point(159, 225), new Point(158, 226), new Point(156, 226), new Point(155, 227), new Point(154, 228), new Point(153, 228), new Point(152, 229), new Point(151, 230), new Point(150, 230), new Point(149, 231), new Point(148, 231), new Point(147, 232), new Point(146, 233), new Point(145, 233), new Point(144, 234), new Point(143, 234), new Point(143, 235), new Point(141, 236), new Point(140, 236), new Point(140, 237), new Point(139, 237), new Point(138, 237), new Point(138, 238), new Point(137, 238), new Point(136, 238), new Point(136, 239), new Point(135, 240), new Point(134, 240), new Point(133, 240), new Point(133, 241), new Point(132, 241), new Point(131, 242), new Point(130, 242), new Point(129, 242), new Point(129, 243), new Point(128, 243), new Point(128, 244), new Point(127, 244), new Point(126, 244), new Point(125, 245), new Point(124, 245), new Point(123, 245), new Point(123, 246), new Point(122, 246), new Point(123, 246) };
        public Point[] debugPoints4 = { new Point(284, 148), new Point(284, 148), new Point(284, 148), new Point(284, 148), new Point(284, 148), new Point(284, 148), new Point(284, 148), new Point(284, 148), new Point(284, 148), new Point(284, 148), new Point(284, 148), new Point(284, 148), };

        
        public List<Tuple<InternalLine, InternalLine, double>> GetSimilarityTestData()
        {
            List<Tuple<InternalLine, InternalLine, double>> list = new List<Tuple<InternalLine, InternalLine, double>>();
            list.Add(new Tuple<InternalLine, InternalLine, double>(
                new InternalLine(new List<Point> { new Point(1,1) }, 0),
                new InternalLine(new List<Point> { new Point(1,1) }, 0),
                1));
            list.Add(new Tuple<InternalLine, InternalLine, double>(
                new InternalLine(new List<Point> { new Point(1, 1) }, 0),
                new InternalLine(new List<Point> { new Point(500, 500) }, 0),
                0.75));
            list.Add(new Tuple<InternalLine, InternalLine, double>(
                new InternalLine(new List<Point> { new Point(1, 1) , new Point(2, 2), new Point(3, 3), new Point(4, 4), new Point(5, 5), new Point(6, 6) }, 0),
                new InternalLine(new List<Point> { new Point(1, 1), new Point(3, 3), new Point(6, 6) }, 0),
                1));
            list.Add(new Tuple<InternalLine, InternalLine, double>(
                new InternalLine(new List<Point> { new Point(1, 1), new Point(2, 2), new Point(3, 3), new Point(4, 4), new Point(5, 5), new Point(6, 6) }, 0),
                new InternalLine(new List<Point> { new Point(301, 301), new Point(303, 303), new Point(306, 306) }, 0),
                0.75));
            list.Add(new Tuple<InternalLine, InternalLine, double>(
                new InternalLine(new List<Point> { new Point(1, 1), new Point(2, 2), new Point(3, 3)}, 0),
                new InternalLine(new List<Point> { new Point(303, 303), new Point(301, 301) }, 0),
                0.75));
            list.Add(new Tuple<InternalLine, InternalLine, double>(
                new InternalLine(new List<Point> { new Point(1, 1), new Point(2, 2), new Point(3, 3) }, 0),
                new InternalLine(new List<Point> { new Point(302, 302), new Point(300, 304) }, 0),
                0.25));
            list.Add(new Tuple<InternalLine, InternalLine, double>(
                new InternalLine(new List<Point> { new Point(1, 1), new Point(1, 2), new Point(1, 3) }, 0),
                new InternalLine(new List<Point> { new Point(301, 2), new Point(330, 2) }, 0),
                0));
            list.Add(new Tuple<InternalLine, InternalLine, double>(
                new InternalLine(new List<Point> { new Point(301, 2), new Point(330, 2) }, 0),
                new InternalLine(new List<Point> { new Point(1, 1), new Point(1, 2), new Point(1, 3) }, 0),
                0));
            list.Add(new Tuple<InternalLine, InternalLine, double>(
                new InternalLine(new List<Point> { new Point(1, 1), new Point(1, 200) }, 0),
                new InternalLine(new List<Point> { new Point(1, 1), new Point(1, 200), new Point(1, 201)}, 0),
                0.996243718592965));

            return list;
        }

        /// <summary>
        /// Generates a random line.
        /// </summary>
        /// <param name="minPoints">The minimum amount of points.</param>
        /// <param name="maxPoints">The maximum amount of points.</param>
        /// <returns>The randomized line.</returns>
        public InternalLine GetRandomLine(uint minPoints, uint maxPoints)
        {
            int len;
            if (minPoints > maxPoints) len = (int) maxPoints;
            else len = rnd.Next((int) minPoints, (int) maxPoints);
            int prevX = rnd.Next(0, 100); int prevY = rnd.Next(0, 100);
            List<Point> points = new List<Point> {new Point(prevX, prevY)};

            for (int i = 0; i < len; i++)
            {
                int opX = rnd.Next(0, 2); int opY = rnd.Next(0, 2);
                int changeX = rnd.Next(0, 50); int changeY = rnd.Next(0, 50);
                if (opX == 0) prevX = prevX - changeX;
                else prevX = prevX + changeX;
                if (opY == 0) prevY = prevY - changeY;
                else prevY = prevY + changeY;
                points.Add(new Point(prevX, prevY));
            }
            return new InternalLine(points,0);
        }
    }
}
