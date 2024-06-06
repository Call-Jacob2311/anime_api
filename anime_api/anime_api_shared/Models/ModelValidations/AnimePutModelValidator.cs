using anime_api_shared.Models.Anime;
using FluentValidation;

namespace anime_api_shared.Models.ModelValidations
{
    public class AnimePutModelValidator : AbstractValidator<AnimePutModel>
    {
        public AnimePutModelValidator(IEnumerable<AnimePutModel> existingAnimes)
        {
            RuleFor(x => x.AnimeId)
                .GreaterThan(0).WithMessage("Anime id count must be greater than 0.");

            RuleFor(x => x.AnimeName)
                .NotEmpty().WithMessage("Anime name is required.")
                .MaximumLength(100).WithMessage("Anime name must not exceed 100 characters.")
                .Must((model, name) => IsUniqueName(model, existingAnimes)).WithMessage("Anime name must be unique.");

            RuleFor(x => x.AnimeStatus)
                .NotEmpty().WithMessage("Anime status is required.")
                .Must(status => status == "Ongoing" || status == "Completed").WithMessage("Anime status must be either 'Ongoing' or 'Completed'.");

            RuleFor(x => x.StudioId)
                .GreaterThan(0).WithMessage("Studio ID must be greater than 0.");

            RuleFor(x => x.ReleaseDate)
                .LessThanOrEqualTo(DateTime.Today).WithMessage("Release date cannot be in the future.");

            RuleFor(x => x.EpisodeCount)
                .GreaterThan(0).WithMessage("Episode count must be greater than 0.");

            RuleFor(x => x.Genres)
                .NotEmpty().WithMessage("Genres are required.");
        }

        private static bool IsUniqueName(AnimePutModel model, IEnumerable<AnimePutModel> existingAnimes)
        {
            return existingAnimes.Count(x => x.AnimeName == model.AnimeName) <= 1;
        }
    }
}
