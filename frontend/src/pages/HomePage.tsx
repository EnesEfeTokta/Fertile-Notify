import { useNavigate } from "react-router-dom";

export default function HomePage() {
    const navigate = useNavigate();

    return (
        <div className="min-h-screen flex flex-col items-center justify-center bg-gray-100">
            <h1 className="text-4xl font-bold text-gray-800">Home Page</h1>
            <div className="mt-6 flex space-x-4">
                <button className="mt-6 px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition"
                    onClick={() => navigate("/login")}>
                    Log In
                </button>
                <button className="mt-6 px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition"
                    onClick={() => navigate("/register")}>
                    Register
                </button>
            </div>
        </div>
    );
}