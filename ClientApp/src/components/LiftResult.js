import React, { useState } from 'react';

const YourResult = () => {
    const [results, setResults] = useState([]);
    const [squat, setSquat] = useState('');
    const [bench, setBench] = useState('');
    const [deadlift, setDeadlift] = useState('');
    const [error, setError] = useState('');

    const handleCompare = (type) => {
        setError('');

        let data = {
            squat: 0,
            bench: 0,
            deadlift: 0,
            total: 0
        };

        if (type === 'squat' && squat) {
            data.squat = parseFloat(squat);
        } else if (type === 'bench' && bench) {
            data.bench = parseFloat(bench);
        } else if (type === 'deadlift' && deadlift) {
            data.deadlift = parseFloat(deadlift);
        } else if (type === 'total') {
            if (squat && bench && deadlift) {
                data.total = parseFloat(squat) + parseFloat(bench) + parseFloat(deadlift);
            } else {
                setError('Please enter values for all lifts to compare total.');
                return;
            }
        } else {
            setError('Please enter a value for the selected lift.');
            return;
        }

        fetch('/LiftResult', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => setResults(data))
            .catch(error => setError('Error posting data: ' + error.message));
    };

    return (
        <div>
            <form onSubmit={(e) => e.preventDefault()}>
                <div>
                    <label>Squat: </label>
                    <input type="number" value={squat} onChange={(e) => setSquat(e.target.value)} step="0.01" />
                    <button type="button" onClick={() => handleCompare('squat')}>Compare Squat</button>
                </div>
                <div>
                    <label>Bench: </label>
                    <input type="number" value={bench} onChange={(e) => setBench(e.target.value)} step="0.01" />
                    <button type="button" onClick={() => handleCompare('bench')}>Compare Bench Press</button>
                </div>
                <div>
                    <label>Deadlift: </label>
                    <input type="number" value={deadlift} onChange={(e) => setDeadlift(e.target.value)} step="0.01" />
                    <button type="button" onClick={() => handleCompare('deadlift')}>Compare Deadlift</button>
                </div>
                <button type="button" onClick={() => handleCompare('total')}>Compare Total</button>
            </form>

            {error && <p style={{ color: 'red' }}>{error}</p>}

            {results && results.length > 0 && (
                <div>
                    <p>Your result:</p>
                    <table>
                        <thead>
                            <tr>
                                <th>Name</th>
                                <th>Squat</th>
                                <th>Benchpress</th>
                                <th>Deadlift</th>
                                <th>Total</th>
                                <th>Sex</th>
                                <th>Age</th>
                                <th>Weight Class</th>
                            </tr>
                        </thead>
                        <tbody>
                            {results.map((result) => (
                                <tr key={result.name}>
                                    <td>{result.name}</td>
                                    <td>{result.squat}</td>
                                    <td>{result.bench}</td>
                                    <td>{result.deadlift}</td>
                                    <td>{result.total}</td>
                                    <td>{result.sex}</td>
                                    <td>{result.age}</td>
                                    <td>{result.weightClass}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}
        </div>
    );
};

export default YourResult;
