using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Validators
{
    public class MovieValidator : AbstractValidator<Movie>
    {
        private readonly IMovieRepository _movieRepository;
        public MovieValidator(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;

            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Genres).NotEmpty();
            RuleFor(x => x.YearOfRelease).LessThanOrEqualTo(DateTime.UtcNow.Year);
            RuleFor(x => x.Slug)
                .MustAsync(ValidateSlug)
                .WithMessage("this movie already exists");
        }

        private async Task<bool> ValidateSlug(Movie movie, string slug, CancellationToken token)
        {
            var exists = await _movieRepository.GetBySlugAsync(slug);

            if (exists is not null) return exists.Id == movie.Id;

            return exists is null;
        }
    }
}