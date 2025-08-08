import React from "react";
import Nav from "../Header/nav";
import Bottom from "../Footer/bottom";
import { useNavigate } from "react-router-dom";



function Forgotpassword() {
    const navigate = useNavigate();
    const handleLogin = () => {
        navigate('');
    }

    return (
        <div className="min-h-screen w-full bg-cover bg-center"
            style={{ backgroundImage: "url('https://images8.alphacoders.com/136/thumb-1920-1368754.jpeg')" }}>
            <div className="sticky top-0 z-50  shadow-md" style={{ backgroundImage: "url('https://th.bing.com/th/id/R.9e8b6083d2c56afe3e37c99a0d008551?rik=MgANzjo9WJbFrA&riu=http%3a%2f%2fgetwallpapers.com%2fwallpaper%2ffull%2f5%2f0%2f3%2f718692-amazing-dark-purple-backgrounds-1920x1200.jpg&ehk=QVn3JWJ991bU4NaIVD9w8hngTuAZ1AHehPjBWxqpDUE%3d&risl=&pid=ImgRaw&r=0')" }}>
                <header>
                    <div className="content-wrapper max-w-screen-xl text-base mx-auto px-8">
                        <Nav />
                    </div>
                </header>
            </div>
            <div className="content-wrapper max-w-screen-2xl text-base mx-auto px-8  min-h-screen ">
                <div>
                    <div className="flex justify-center items-center h-full w-full pt-24">
                        <div className="grid gap-8 border-4 border-double border-indigo-700 backdrop-blur-none">
                            <section
                                id="back-div"
                                className="bg-transparent  rounded-3xl">
                                <div
                                    className="border-8 border-transparent rounded-xl  shadow-xl p-8 m-2">
                                    <h2
                                        className="text-xl pb-10 font-bold text-center cursor-default dark:text-gray-300 text-gray-900">
                                        THAY ĐỔI MẬT KHẨU
                                    </h2>
                                    <form action="#" method="post" className="space-y-6">
                                        <div>
                                            <label className="flex font-semibold text-gray-50 justify-start items-start mb-2 text-base dark:text-gray-300 px-4 ">Mật khẩu cũ</label>
                                            <input
                                                id="password"
                                                className="border p-3 shadow-md bg-transparent dark:text-gray-300 dark:border-gray-700 border-gray-300 rounded-xl w-[420px] focus:ring-2 focus:ring-blue-500 transition transform hover:scale-105 duration-300"
                                                type="password"
                                                placeholder="Mật khẩu cũ"
                                            />
                                        </div>
                                        <div>
                                            <label className="flex font-semibold text-gray-50 justify-start items-start mb-2 text-base dark:text-gray-300 px-4 ">Mật khẩu mới</label>
                                            <input
                                                id="password"
                                                className="border p-3  shadow-md bg-transparent dark:text-gray-300 dark:border-gray-700 border-gray-300 rounded-xl w-[420px] focus:ring-2 focus:ring-blue-500 transition transform hover:scale-105 duration-300"
                                                type="password"
                                                placeholder="Mật khẩu mới"
                                            />
                                        </div>
                                        <div className="pb-10">
                                            <label className="flex font-semibold text-gray-50 justify-start items-start mb-2 text-base dark:text-gray-300 px-4 ">Nhập lại mật khẩu mới</label>
                                            <input
                                                id="password"
                                                className="border p-3 shadow-md bg-transparent dark:text-gray-300 dark:border-gray-700 border-gray-300 rounded-xl w-[420px] focus:ring-2 focus:ring-blue-500 transition transform hover:scale-105 duration-300"
                                                type="password"
                                                placeholder="Nhập lại mật khẩu mới"
                                            />
                                        </div>
                                        <button
                                            className="w-full text-xl font-bold p-3 mt-4 text-white bg-gradient-to-r from-blue-500 to-purple-500 rounded-xl hover:scale-105 transition transform duration-300 shadow-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            type="submit"
                                            onClick={handleLogin}>
                                            CẬP NHẬT PASS
                                        </button>
                                    </form>
                                </div>
                            </section>
                        </div>
                    </div>
                </div>
            </div>
            <footer className="pt-32">
                <Bottom />
            </footer>
        </div>
    );
}
export default Forgotpassword;