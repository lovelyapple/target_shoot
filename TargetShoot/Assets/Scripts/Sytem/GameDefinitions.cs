namespace GameDefinition
{
    public enum SceneType
    {
        Transition_Internal,
        Title,
        Game,
    }
    public enum FieldTargetType
    {
        ScoreTarget,
        CharaTarget,
    }
    public static class GameConstant
    {
        public const int ApplicationVersion = 1;
        public const float MoveLimit = 3f;
        public const float MoveSpeed = 2f;
    }
}