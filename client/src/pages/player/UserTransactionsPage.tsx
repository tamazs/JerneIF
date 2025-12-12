import {useEffect, useState} from "react";
import {useAtom} from "jotai";
import {useNavigate} from "react-router";

import useJerneCrud from "../../hooks/useJerneCrud.ts";
import {transactionsAtom} from "../../atoms/jerneAtom.ts";


export default function UserTransactionsPage() {
    const [status, setStatus] = useState("");
    const [page, setPage] = useState(1);
    const navigate = useNavigate();

    const [transactions] = useAtom(transactionsAtom);

    const jerneCrud = useJerneCrud();

    function buildSieveModel() {
        const filters = [];

        if (status) filters.push(`status==${status}`);

        return {
            filters: filters.join(","),
            sorts: "-createdAt",
            page,
            pageSize: 5
        };
    }

    useEffect(() => {
        const sieve = buildSieveModel();
        jerneCrud.getTransactionsForPlayer(sieve);
    }, [status, page]);


    return (
        <div className="overflow-x-auto flex flex-col">
            <div className="flex justify-center">
                <div className="flex w-full justify-start items-center gap-12 mb-4">
                    <form className="filter">
                        <input className="btn btn-square" type="reset" value="×" onClick={() => setStatus("")}/>
                        <input className="btn" type="radio" name="frameworks" aria-label="Pending" onClick={() => setStatus("Pending")}/>
                        <input className="btn" type="radio" name="frameworks" aria-label="Approved" onClick={() => setStatus("Approved")}/>
                        <input className="btn" type="radio" name="frameworks" aria-label="Rejected" onClick={() => setStatus("Rejected")}/>
                    </form>
                </div>
                <div>
                    <button className="btn btn-primary" onClick={() => navigate("/addtransaction")}>Add Transaction</button>
                </div>
            </div>
            <table className="table mb-4">
                {/* head */}
                <thead>
                <tr>
                    <th>MobilePay Reference</th>
                    <th>Amount</th>
                    <th>Status</th>
                    <th>Created At</th>
                </tr>
                </thead>
                <tbody>
                {transactions.map((transaction) => {
                    return (
                        <tr className="hover:bg-base-300" key={transaction.transactionId}>
                            <td>{transaction.mobilePayReference}</td>
                            <td>{transaction.amount} DKK</td>
                            <td>
                                {transaction.status === "Pending" && (
                                    <div className="badge badge-warning">Pending</div>
                                )}
                                {transaction.status === "Approved" && (
                                    <div className="badge badge-success">Approved</div>
                                )}
                                {transaction.status === "Rejected" && (
                                    <div className="badge badge-error">Rejected</div>
                                )}
                            </td>
                            <td>{new Date(transaction.createdAt).toLocaleString("da-DK", {
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