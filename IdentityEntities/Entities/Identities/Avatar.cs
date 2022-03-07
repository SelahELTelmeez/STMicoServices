using CurriculumEntites.Entities.Shared;
using IdentityEntities.Entities.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityEntities.Entities.Identities
{
    public class Avatar : BaseEntity
    {
        public AvatarType AvatarType { get; set; }
        public string Image { get; set; }
    }
}

/*
 * 
 * https://selaheltelmeez.com/Media21-22/LMSApp/avatar/+UserType+img
 * 
 * 
 * 
0	default	default.png
1	Student	01.png
2	Student	02.png
3	Student	03.png
4	Student	04.png
5	Student	05.png
6	Student	06.png
7	Student	07.png
8	Student	08.png
11	Parent	01.png
12	Parent	02.png
13	Parent	03.png
14	Parent	04.png
15	Parent	05.png
16	Parent	06.png
17	Parent	07.png
18	Parent	08.png
19	Teacher	01.png
20	Teacher	02.png
21	Teacher	03.png
22	Teacher	04.png
23	Teacher	05.png
24	Teacher	06.png
25	Teacher	07.png
26	Teacher	08.png
27	Student	09.png
28	Student	10.png
29	Student	11.png
30	Student	12.png
31	Student	13.png
32	Student	14.png
33	Student	15.png
34	Student	16.png
35	Teacher	09.png
36	Teacher	10.png
37	Teacher	11.png
38	Teacher	12.png
39	Teacher	13.png
40	Teacher	14.png
41	Teacher	15.png
42	Teacher	16.png
 * 
 * */