namespace CurriculumEntites.Entities.Shared;
public enum CurriculumStage { KG = 0, Primary = 1, Preparatory = 2, HighSchool = 3 }
public enum ClipStatus { UnderPreparation = 0, Testing = 1, Ready = 2, Production = 3 }
public enum Orientation { Landscape = 1, Portrait = 2 }
public enum FormType { Text, Image }
public enum ClipType
{
    FlipHtml = 0,
    PDF = 8,
    YouTube = 4,
    Quiz = 9,
    Exercise = 11,
    Dictation = 12,
    ExerciseMCQ = 13,
}
