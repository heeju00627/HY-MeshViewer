fid = fopen('..\defaultmodel.bin','w');
fwrite(fid, blanks(100), 'char'); %subject name
fwrite(fid, 1, 'int32'); %surface flag
fwrite(fid, 1, 'int32'); %electrode flag
% volume data
fwrite(fid, elec.head.nnode, 'int32'); %nnode
fwrite(fid, elec.head.nele, 'int32'); %nele
fwrite(fid, elec.head.nodes, 'double'); %nodes (nnode * 3 )
fwrite(fid, elec.head.elements, 'int32'); %elements (nele * 4)
fwrite(fid, elec.head.regions, 'int32'); %regions
%surface data - scalp
fwrite(fid, elec.scalp.nnode, 'int32'); %nnode
fwrite(fid, elec.scalp.nele, 'int32'); %nele
fwrite(fid, elec.scalp.nodes, 'double'); %nodes (nnode*3)
fwrite(fid, elec.scalp.elements, 'int32'); %elements (nele*3)
fwrite(fid, elec.scalp.trace, 'int32'); %regions
%surface data - cortex
fwrite(fid, elec.cortex.nnode, 'int32'); %nnode
fwrite(fid, elec.cortex.nele, 'int32'); %nele
fwrite(fid, elec.cortex.nodes, 'double'); %nodes (nnode*3)
fwrite(fid, elec.cortex.elements, 'int32'); %elements (nele*3)
fwrite(fid, elec.cortex.trace, 'int32'); %regions
%electrodes
fwrite(fid, length(elec.elecSeedNodes), 'int32'); %length of list of all nodes in all electrodes
fwrite(fid, elec.elecSeedNodes, 'int32'); %list of all nodes in all electrodes
fwrite(fid, elec.nelectrode, 'int32'); %nelectrode
for i=1:elec.nelectrode
    fwrite(fid, size(elec.elecInterface,1), 'int32'); %the number of faces between scalp and each electrode
end
for i=1:elec.nelectrode
    fwrite(fid, (elec.elecInterface{i}), 'int32'); %faces between scalp and each electrode (ninterface(i)*4)
                                                %'node1 node2 node3 element' - node numbers of face, element number of the face 
end

fclose(fid);