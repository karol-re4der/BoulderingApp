namespace BoulderBuddy.Models.ViewModels
{
    public class RouteViewModel(Routes route, List<CommentsViewModel> comments, List<GradeRatings> ratings)
    {
        public Routes Route { get; set; } = route;
        public List<CommentsViewModel> RouteComments { get; set; } = comments;
        public List<GradeRatings> GradeRatings { get; set; } = ratings;

        #region Progress Bar
        public int Progress_Grading_Easy = 0;
        public int Progress_Grading_Fair = 0;
        public int Progress_Grading_Hard = 0;
        #endregion

        public AscentsSectionViewModel AscentsSectionViewModel { get; set; }
    }
}
