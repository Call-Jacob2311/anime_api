using anime_api_shared.Models.Anime;
using FluentValidation;

namespace anime_api_shared.Models.ModelValidations
{
    public class AnimePostModelValidator : AbstractValidator<AnimePostModel>
    {
        public AnimePostModelValidator(IEnumerable<AnimePostModel> existingAnimes)
        {
            RuleFor(x => x.AnimeName)
                .NotEmpty().WithMessage("Anime name is required.")
                .MaximumLength(100).WithMessage("Anime name must not exceed 100 characters.")
                .Must((model, name) => IsUniqueName(model, existingAnimes)).WithMessage("Anime name must be unique.");

            RuleFor(x => x.AnimeStatusId)
                .GreaterThan(0).WithMessage("Studio ID must be greater than 0.");

            RuleFor(x => x.StudioId)
                .GreaterThan(0).WithMessage("Studio ID must be greater than 0.");

            RuleFor(x => x.ReleaseDate)
                .LessThanOrEqualTo(DateTime.Today).WithMessage("Release date cannot be in the future.");

            RuleFor(x => x.EpisodeCount)
                .GreaterThan(0).WithMessage("Episode count must be greater than 0.");

            RuleFor(x => x.Genres)
                .NotEmpty().WithMessage("Genres are required.");

            RuleFor(x => x.AnimeOSTId)
                .GreaterThan(0).WithMessage("Episode count must be greater than 0.");
        }

        private static bool IsUniqueName(AnimePostModel model, IEnumerable<AnimePostModel> existingAnimes)
        {
            return existingAnimes.Count(x => x.AnimeName == model.AnimeName) <= 1;
        }
    }
}