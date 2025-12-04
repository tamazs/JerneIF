import {finalUrl} from "../baseUrl.ts";
import {useAtom} from "jotai";
import toast from "react-hot-toast";
import customcatch from "../errors/customcatch.ts";
import {
    AuthClient,
    UserClient,
    type LoginRequestDto,
    type RegisterRequestDto,
    type UpdateUserRequestDto, type CreateTransactionRequestDto, TransactionClient, type UserDto, BoardClient,
    type AddBoardRequestDto,
} from "../generated-ts-client.ts";
import {
    loggedInUserAtom,
    accessTokenAtom,
    refreshTokenAtom,
    usersAtom,
    transactionsAtom,
    balanceAtom, boardsAtom
} from "../atoms/jerneAtom.ts";
import {useNavigate} from "react-router";
import {customFetch} from "./customFetch.ts";

const authClient = new AuthClient(finalUrl);
const userClient = new UserClient(finalUrl, customFetch);
const transactionClient = new TransactionClient(finalUrl, customFetch);
const boardClient = new BoardClient(finalUrl, customFetch);

export default function useJerneCrud() {
    const navigate = useNavigate();

    const [users, setUsers] = useAtom(usersAtom);
    const [loggedInUser, setLoggedInUser] = useAtom(loggedInUserAtom);
    const [accessToken, setAccessToken] = useAtom(accessTokenAtom);
    const [refreshToken, setRefreshToken] = useAtom(refreshTokenAtom);
    const [balance, setBalance] = useAtom(balanceAtom);
    const [boards, setBoards] = useAtom(boardsAtom);

    const [transactions, setTransactions] = useAtom(transactionsAtom);

    async function loginUser(dto : LoginRequestDto) {
        try {
            const result = await authClient.loginUser(dto)
            // @ts-ignore
            setLoggedInUser(result.user ?? null)
            setAccessToken(result.token)
            setRefreshToken(result.refreshToken)

            localStorage.setItem("accessToken", result.token);
            localStorage.setItem("refreshToken", result.refreshToken);
            localStorage.setItem("user", JSON.stringify(result.user));
            await getBalance();

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
            navigate(-1);
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

    async function editUser(dto: UpdateUserRequestDto) {
        try {
            const result = await userClient.updateUser(dto);
            const index = users.findIndex(u => u.userId === result.userId);

            if (index > -1) {
                const duplicate = [...users];
                duplicate[index] = result;
                setUsers(duplicate);
                toast.success("Update successful!");
                navigate(-1);
            }
        } catch (e) {
            customcatch(e);
        }
    }

    async function deleteUser(userId: string) {
        try {
            await userClient.deleteUser(userId);

            setUsers(prev =>
                prev.map(u =>
                    u.userId === userId ? { ...u, isActive: false } : u
                )
            );

            toast.success("User deactivated!");
        } catch (e) {
            customcatch(e);
        }
    }

    async function logoutUser() {
        setAccessToken(null);
        setRefreshToken(null);
        // @ts-ignore
        setLoggedInUser(null);

        localStorage.removeItem("accessToken");
        localStorage.removeItem("refreshToken");
        localStorage.removeItem("user");

        navigate("/login");
    }

    async function addTransaction(dto: CreateTransactionRequestDto) {
        try {
            const result = await transactionClient.createTransaction(dto);
            const duplicate = [...transactions];
            duplicate.push(result);
            setTransactions(duplicate);
            navigate(-1);
            toast.success("Transaction request sent!");
        } catch (e) {
            customcatch(e);
        }
    }

    async function getTransactions(sieve: any) {
        try {
            const result = await transactionClient.getTransactions(sieve);
            setTransactions(result);
        } catch (e) {
            customcatch(e);
        }
    }

    async function getTransactionsForPlayer(sieve: any) {
        try {
            const result = await transactionClient.getTransactionsByUserId(sieve);
            setTransactions(result);
        } catch (e) {
            customcatch(e);
        }
    }

    async function approveTransaction(transactionId: string) {
        try {
            await transactionClient.approveTransaction(transactionId);
            setTransactions(prev =>
                prev.map(t =>
                    t.transactionId === transactionId ? { ...t, status: "Approved" } : t
                )
            );

            toast.success("Transaction approved!");
        } catch (e) {
            customcatch(e);
        }
    }

    async function rejectTransaction(transactionId: string) {
        try {
            await transactionClient.denyTransaction(transactionId);
            setTransactions(prev =>
                prev.map(t =>
                    t.transactionId === transactionId ? { ...t, status: "Rejected" } : t
                )
            );

            toast.success("Transaction rejected!");
        } catch (e) {
            customcatch(e);
        }
    }

    async function getBalance() {
        try {
            const result = await boardClient.getBalance();
            setBalance(result);
        } catch (e) {
            customcatch(e);
        }
    }

    async function addBoard(dto: AddBoardRequestDto) {
        try {
            const result = await boardClient.createBoard(dto);
            const duplicate = [...boards];
            duplicate.push(result);
            setBoards(duplicate);
            //navigate(-1);
            await getBalance();
            toast.success("Board added successfully!");
        } catch (e) {
            customcatch(e);
        }
    }

    return {
        loginUser,
        logoutUser,
        registerUser,
        getAllUsers,
        deleteUser,
        editUser,
        addTransaction,
        getTransactions,
        approveTransaction,
        rejectTransaction,
        getTransactionsForPlayer,
        getBalance,
        addBoard,
    }
}