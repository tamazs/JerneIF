import {useEffect, useState} from "react";
import {useAtom} from "jotai";
import {useNavigate} from "react-router";

import useJerneCrud from "../../hooks/useJerneCrud.ts";
import {boardsAtom} from "../../atoms/jerneAtom.ts";


export default function PlayerBoardList() {
    const [page, setPage] = useState(1);
    const navigate = useNavigate();

    const [boards] = useAtom(boardsAtom);

    const jerneCrud = useJerneCrud();

    function buildSieveModel() {

        return {
            filters: "",
            sorts: "-purchasedAt",
            page,
            pageSize: 5
        };
    }

    useEffect(() => {
        const sieve = buildSieveModel();
        jerneCrud.getPlayerBoards(sieve);
    }, [page]);


    return (
        <div className="overflow-x-auto flex flex-col">
                <div className="flex justify-end items-center">
                    <button className="btn btn-primary" onClick={() => navigate("/addboard")}>Add Board</button>
                </div>
            <table className="table mb-4">
                {/* head */}
                <thead>
                <tr>
                    <th>Numbers</th>
                    <th>Price</th>
                    <th>Repeating for (weeks)</th>
                    <th>Purchased At</th>
                </tr>
                </thead>
                <tbody>
                {boards.map((board) => {
                    return (
                        <tr className="hover:bg-base-300" key={board.boardId}>
                            <td>{board.boardNumbers.join(", ")}</td>
                            <td>{board.price} DKK</td>
                            <td>{board.repeatCount}</td>
                            <td>{new Date(board.purchasedAt).toLocaleString("da-DK", {
                                timeZone: "Europe/Copenhagen",
                                year: "numeric",
                                month: "2-digit",
                                day: "2-digit",
                                hour: "2-digit",
                                minute: "2-digit",
                            })}</td>
                        </tr>
                    )
                })}
                </tbody>
            </table>
            <div className="join grid grid-cols-2 gap-4 self-center">
                <button className="join-item btn btn-outline" onClick={() => setPage(page - 1)} disabled={page === 1}>Previous page</button>
                <button className="join-item btn btn-outline" onClick={() => setPage(page + 1)}>Next page</button>
            </div>
        </div>
    )
}