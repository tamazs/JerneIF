import {atom} from "jotai";

import type {BoardDto, TransactionDto, UserDto} from "../generated-ts-client.ts";

export const loggedInUserAtom = atom<UserDto | null>(JSON.parse(localStorage.getItem("user") || "null"));
export const usersAtom = atom<UserDto[]>([]);
export const transactionsAtom = atom<TransactionDto[]>([]);
export const balanceAtom = atom<number>(0);
export const boardsAtom = atom<BoardDto[]>([]);

export const accessTokenAtom = atom<string | null>(localStorage.getItem("accessToken"));
export const refreshTokenAtom = atom<string | null>(localStorage.getItem("refreshToken"));