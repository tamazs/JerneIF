import {atom} from "jotai";

import type {UserDto} from "../generated-ts-client.ts";

export const loggedInUserAtom = atom<UserDto | null>(null);
export const usersAtom = atom<UserDto[]>([]);

export const accessTokenAtom = atom<string | null>(null);
export const refreshTokenAtom = atom<string | null>(null);