import {atom} from "jotai";

import type {TransactionDto, UserDto} from "../generated-ts-client.ts";

export const loggedInUserAtom = atom<UserDto | null>(JSON.parse(localStorage.getItem("user") || "null"));
export const usersAtom = atom<UserDto[]>([]);
export const transactionsAtom = atom<TransactionDto[]>([]);

export const accessTokenAtom = atom<string | null>(null);
export const refreshTokenAtom = atom<string | null>(null);