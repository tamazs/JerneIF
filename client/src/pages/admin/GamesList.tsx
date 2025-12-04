import {useEffect, useState} from "react";
import useJerneCrud from "../../hooks/useJerneCrud.ts";
import {useAtom} from "jotai";
import {gamesAtom} from "../../atoms/jerneAtom.ts";
import {useNavigate} from "react-router";

export default function GamesList() {
    const [page, setPage] = useState(1);
    const navigate = useNavigate();

    const [games] = useAtom(gamesAtom);

    const jerneCrud = useJerneCrud();

    function buildSieveModel() {

        return {
            filters: "",
            sorts: "-createdAt",
            page,
            pageSize: 5
        };
    }

    useEffect(() => {
        const sieve = buildSieveModel();
        jerneCrud.getGames(sieve);
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
                    <th>Winning Numbers</th>
                    <th>Start Date</th>
                    <th>Status</th>
                    <th>Published at</th>
                    <th>Published by</th>
                    <th>Winners</th>
                </tr>
                </thead>
                <tbody>
                {games.map((game) => {
                    return (
                        <tr className="hover:bg-base-300" key={game.gameId}>
                            <td>{game.gameWinningNumber ? game.gameWinningNumber?.join(", ") : "Not published yet"}</td>
                            <td>{new Date(game.startDate).toLocaleString("da-DK", {
                                timeZone: "Europe/Copenhagen",
                                year: "numeric",
                                month: "2-digit",
                                day: "2-digit",
                                hour: "2-digit",
                                minute: "2-digit",
                            })}</td>
                            <td>
                                {game.status === "Active" && (
                                    <div className="badge badge-success">Active</div>
                                )}
                                {game.status === "Finished" && (
                                    <div className="badge badge-error">Finished</div>
                                )}
                            </td>
                            <td>
                                {game.publishedAt
                                    ? new Date(game.publishedAt).toLocaleString("da-DK", {
                                        timeZone: "Europe/Copenhagen",
                                        year: "numeric",
                                        month: "2-digit",
                                        day: "2-digit",
                                        hour: "2-digit",
                                        minute: "2-digit",
                                    })
                                    : "Not published yet"}
                            </td>
                            <td>{game.publishedByUser?.fullName}</td>
                            <td>
                                {game.winners && game.winners.length > 0 ? (
                                    <ul className="list-disc ml-4">
                                        {game.winners.map((winner, index) => (
                                            <li key={index}>
                                                <strong>{winner.fullName}</strong> – {winner.matchedNumbers.join(", ")}
                                            </li>
                                        ))}
                                    </ul>
                                ) : (
                                    "No winners yet"
                                )}
                            </td>
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