import type {ProblemDetails} from "./problemdetails.ts";
import {ApiException} from "../generated-ts-client.ts";
import toast from "react-hot-toast";

export default function customcatch(e : any) {
    if (e instanceof ApiException) {
        const problemDetails = JSON.parse(e.response) as ProblemDetails;
        toast.error(problemDetails.title);
    }
}