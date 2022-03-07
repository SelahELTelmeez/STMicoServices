using IdentityEntities.Entities.Shared;

namespace IdentityEntities.Entities.Locations
{
    public class Governorate : BaseEntity
    {
        public string Name { get; set; }
        public bool IsEnabled { get; set; }
    }
}
/*
 * 
 * 0	
True	NULL	NULL
1	القاهرة	True	
2	الجيزة	True	
3	حلوان	False	
4	الدقهلية	True	
5	المنوفية	True	
6	الاسكندرية	True	
7	الشرقية	True	
8	الغربية	True	
9	القليوبية	True	
10	بور سعيد	True	
11	اسوان	True	
21	6 أكتوبر	False	
25	اسيوط	True	NULL	NULL
32	كفر الشيخ	True	NULL	NULL
33	السويس	True	NULL	NULL
34	بنى سويف	True	NULL	NULL
35	الفيوم	True	NULL	NULL
36	البحيرة	True	NULL	NULL
37	المنيا	True	NULL	NULL
39	سوهاج	True	NULL	NULL
42	الاسماعيلية	True	NULL	NULL
43	شمال سيناء	True	NULL	NULL
44	دمياط	True	NULL	NULL
47	الاقصر	True	NULL	NULL
48	جنوب سيناء	True	NULL	NULL
50	البحر الاحمر	True	NULL	NULL
51	قنا	True	NULL	NULL
54	الوادى الجديد	True	NULL	NULL
55	مرسى مطروح	True
 * 
 */