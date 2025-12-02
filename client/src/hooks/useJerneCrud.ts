import {finalUrl} from "../baseUrl.ts";
import {useAtom} from "jotai";
import toast from "react-hot-toast";
import customcatch from "../errors/customcatch.ts";
import {AuthClient, UserClient, type LoginRequestDto, type RegisterRequestDto,} from "../generated-ts-client.ts";
import {loggedInUserAtom, accessTokenAtom, refreshTokenAtom, usersAtom} from "../atoms/jerneAtom.ts";
import {useNavigate} from "react-router";
import {customFetch} from "./customFetch.ts";

const authClient = new AuthClient(finalUrl);
const userClient = new UserClient(finalUrl, customFetch);

export default function useJerneCrud() {
    const navigate = useNavigate();

    const [users, setUsers] = useAtom(usersAtom);
    const [loggedInUser, setLoggedInUser] = useAtom(loggedInUserAtom);
    const [accessToken, setAccessToken] = useAtom(accessTokenAtom);
    const [refreshToken, setRefreshToken] = useAtom(accessTokenAtom);

    async function loginUser(dto : LoginRequestDto) {
        try {
            const result = await authClient.loginUser(dto)
            setLoggedInUser(result.user)
            setAccessToken(result.token)
            setRefreshToken(result.refreshToken)

            localStorage.setItem("accessToken", result.token);
            localStorage.setItem("refreshToken", result.refreshToken);
            localStorage.setItem("user", JSON.stringify(result.user));

            toast.success("Login successful!");

            if (result.user.role === "Admin") {
                navigate("/admin");
            } else {
                navigate("/");
            }
        } catch (e) {
            customcatch(e)
        }
    }

    async function registerUser(dto: RegisterRequestDto) {
        try {
            const result = await authClient.registerUser(dto);
            const duplicate = [...users];
            duplicate.push(result);
            setUsers(duplicate);
            toast.success("Register successful!");
            return result;
        }
        catch (e) {
            customcatch(e);
        }
    }

    async function getAllUsers(sieve: any) {
        try {
            const result = await userClient.getUsers(sieve);
            setUsers(result);
        } catch (e) {
            customcatch(e);
        }
    }

    async function logoutUser() {
        setAccessToken(null);
        setRefreshToken(null);
        setLoggedInUser(null);

        localStorage.removeItem("accessToken");
        localStorage.removeItem("refreshToken");
        localStorage.removeItem("user");

        navigate("/login");
    }

    return {
        loginUser,
        logoutUser,
        registerUser,
        getAllUsers
    }
}