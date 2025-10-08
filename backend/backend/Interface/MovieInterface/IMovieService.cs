﻿using backend.Model.Movie;
using backend.ModelDTO.MoviesDTO.MovieRequest;
using backend.ModelDTO.MoviesDTO.MovieRespond;
using Microsoft.EntityFrameworkCore.Update.Internal;
using backend.ModelDTO.PaginiationDTO.Respond;
using backend.ModelDTO.GenericRespond;
using backend.ModelDTO.MoviesDTO;

namespace backend.Interface.MovieInterface
{
    public interface IMovieService
    {
        // Thêm phim
        Task<GenericRespondDTOs> add(MovieRequestDTO entity);

        // Xóa phim
        Task<GenericRespondDTOs> remove(string Id);

        // CHỉnh sửa phim

        Task<GenericRespondDTOs> edit(string movieID, MovieEditRequestDTO dtos);

        GenericRespondWithObjectDTO<movieGetDetailResponseDTO> getMovieDetail(string movieID);

 
        // Cơ chế phân trang để giúp giảm tải ở bên phía server (trang 1 lấy data 1,2,3 - Trang 2 lấy data 4,5,6)

        Task<PagniationRespond> getListItemsPagination(int page , int pagesize = 9);

        // Áp dụng pagination giảm lưu lượng cho bên API
        // Lấy 5 phim bât kì khi tìm kiếm

        Task<List<movieRespondDTO>> getListMoviesByNameTake5(string movie);

        // Trường hợp người dùng ân vào xem thêm khi tìm kiếm phim xong thì sẽ hiển thị 
        // hết danh sách
        Task<PagniationRespond> getFullSearchResult(string movieName ,int page, int pagesize = 9);

        // Lấy thông tin phim khi ấn vào tìm kiếm theo ID

        //Task<List<movieRespondDTO>> getMovie(string movieID);

        Task<GenericRespondWithObjectDTO<List<GetMovieShowedDTOList>>> GetShowedMovieTake5();
        
        Task<GenericRespondWithObjectDTO<List<GetMovieShowedDTOList>>> GetUnShowedMovieTake5();
    }
}
